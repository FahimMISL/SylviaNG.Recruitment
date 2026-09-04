using System.Text;

namespace SylviaNG.Recruitment.Application.Common.BooleanQuery
{
    /// <summary>
    /// Parses a Boolean CV Bank search query (US-045 AC1/AC2) into an evaluable expression tree.
    /// Supported syntax: AND / OR / NOT (case-insensitive), parentheses for grouping, quoted
    /// "multi word phrases", and implicit AND between adjacent bare terms (e.g. `react node` ==
    /// `react AND node`). Precedence, tightest first: NOT, AND, OR - parentheses override.
    /// Evaluation is a plain case-insensitive substring match against a haystack string, the
    /// same semantics as the existing Contains-based search in PaginationExtensions.ApplySearch.
    /// </summary>
    public static class BooleanQueryParser
    {
        public static IBooleanQueryNode Parse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query must not be empty.", nameof(query));

            var tokens = Tokenize(query);
            if (tokens.Count == 0)
                throw new ArgumentException("Query must contain at least one search term.", nameof(query));

            var position = 0;
            var node = ParseOr(tokens, ref position);

            if (position != tokens.Count)
                throw new FormatException($"Unexpected token '{tokens[position].Text}' in Boolean query.");

            return node;
        }

        /// <summary>Every literal term/phrase in the tree, for relevance scoring.</summary>
        public static int CountMatchedTerms(IBooleanQueryNode node, string haystackLower)
        {
            var terms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            node.CollectTerms(terms);
            return terms.Count(t => haystackLower.Contains(t, StringComparison.OrdinalIgnoreCase));
        }

        // ── Grammar ──────────────────────────────────────────────────────
        // Or   := And (OR And)*
        // And  := Not (AND? Not)*        (implicit AND when no operator token is present)
        // Not  := NOT Not | Primary
        // Primary := TERM | PHRASE | '(' Or ')'

        private static IBooleanQueryNode ParseOr(List<Token> tokens, ref int position)
        {
            var left = ParseAnd(tokens, ref position);

            while (Peek(tokens, position)?.Kind == TokenKind.Or)
            {
                position++;
                var right = ParseAnd(tokens, ref position);
                left = new OrNode(left, right);
            }

            return left;
        }

        private static IBooleanQueryNode ParseAnd(List<Token> tokens, ref int position)
        {
            var left = ParseNot(tokens, ref position);

            while (true)
            {
                var next = Peek(tokens, position);
                if (next == null || next.Kind == TokenKind.Or || next.Kind == TokenKind.RParen)
                    break;

                if (next.Kind == TokenKind.And)
                    position++; // explicit AND - consume it
                // else: implicit AND between two adjacent terms - do not consume, just recurse

                var right = ParseNot(tokens, ref position);
                left = new AndNode(left, right);
            }

            return left;
        }

        private static IBooleanQueryNode ParseNot(List<Token> tokens, ref int position)
        {
            if (Peek(tokens, position)?.Kind == TokenKind.Not)
            {
                position++;
                return new NotNode(ParseNot(tokens, ref position));
            }

            return ParsePrimary(tokens, ref position);
        }

        private static IBooleanQueryNode ParsePrimary(List<Token> tokens, ref int position)
        {
            var token = Peek(tokens, position)
                ?? throw new FormatException("Unexpected end of Boolean query.");

            switch (token.Kind)
            {
                case TokenKind.LParen:
                    position++;
                    var inner = ParseOr(tokens, ref position);
                    if (Peek(tokens, position)?.Kind != TokenKind.RParen)
                        throw new FormatException("Missing closing ')' in Boolean query.");
                    position++;
                    return inner;

                case TokenKind.Term:
                case TokenKind.Phrase:
                    position++;
                    return new TermNode(token.Text);

                default:
                    throw new FormatException($"Unexpected token '{token.Text}' in Boolean query.");
            }
        }

        private static Token? Peek(List<Token> tokens, int position) =>
            position < tokens.Count ? tokens[position] : null;

        // ── Tokenizer ────────────────────────────────────────────────────

        private enum TokenKind { Term, Phrase, And, Or, Not, LParen, RParen }

        private sealed record Token(TokenKind Kind, string Text);

        private static List<Token> Tokenize(string query)
        {
            var tokens = new List<Token>();
            var i = 0;

            while (i < query.Length)
            {
                var c = query[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c is '(' or ')')
                {
                    tokens.Add(new Token(c == '(' ? TokenKind.LParen : TokenKind.RParen, c.ToString()));
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    var sb = new StringBuilder();
                    i++;
                    while (i < query.Length && query[i] != '"')
                        sb.Append(query[i++]);
                    if (i < query.Length) i++; // closing quote
                    if (sb.Length > 0)
                        tokens.Add(new Token(TokenKind.Phrase, sb.ToString()));
                    continue;
                }

                // Bare word: letters/digits/most punctuation except parens/quotes.
                var start = i;
                while (i < query.Length && query[i] is not ('(' or ')' or '"') && !char.IsWhiteSpace(query[i]))
                    i++;

                var word = query[start..i];
                tokens.Add(word.ToUpperInvariant() switch
                {
                    "AND" => new Token(TokenKind.And, word),
                    "OR" => new Token(TokenKind.Or, word),
                    "NOT" => new Token(TokenKind.Not, word),
                    _ => new Token(TokenKind.Term, word)
                });
            }

            return tokens;
        }
    }

    public interface IBooleanQueryNode
    {
        bool Evaluate(string haystackLower);
        void CollectTerms(HashSet<string> terms);
    }

    public sealed class TermNode : IBooleanQueryNode
    {
        private readonly string _term;

        public TermNode(string term) => _term = term;

        public bool Evaluate(string haystackLower) => haystackLower.Contains(_term, StringComparison.OrdinalIgnoreCase);

        public void CollectTerms(HashSet<string> terms) => terms.Add(_term);
    }

    public sealed class AndNode : IBooleanQueryNode
    {
        private readonly IBooleanQueryNode _left;
        private readonly IBooleanQueryNode _right;

        public AndNode(IBooleanQueryNode left, IBooleanQueryNode right) { _left = left; _right = right; }

        public bool Evaluate(string haystackLower) => _left.Evaluate(haystackLower) && _right.Evaluate(haystackLower);

        public void CollectTerms(HashSet<string> terms) { _left.CollectTerms(terms); _right.CollectTerms(terms); }
    }

    public sealed class OrNode : IBooleanQueryNode
    {
        private readonly IBooleanQueryNode _left;
        private readonly IBooleanQueryNode _right;

        public OrNode(IBooleanQueryNode left, IBooleanQueryNode right) { _left = left; _right = right; }

        public bool Evaluate(string haystackLower) => _left.Evaluate(haystackLower) || _right.Evaluate(haystackLower);

        public void CollectTerms(HashSet<string> terms) { _left.CollectTerms(terms); _right.CollectTerms(terms); }
    }

    public sealed class NotNode : IBooleanQueryNode
    {
        private readonly IBooleanQueryNode _operand;

        public NotNode(IBooleanQueryNode operand) => _operand = operand;

        public bool Evaluate(string haystackLower) => !_operand.Evaluate(haystackLower);

        // A NOT-only term shouldn't count toward relevance (it's an exclusion, not a match signal).
        public void CollectTerms(HashSet<string> terms) { }
    }
}
