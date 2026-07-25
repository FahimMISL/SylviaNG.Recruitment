namespace SylviaNG.Recruitment.Infrastructure.SeedData
{
    /// <summary>
    /// Static Bangladesh administrative geography (Division -&gt; District -&gt; Thana/Upazila) used to
    /// seed the candidate profile address dropdowns. Best-effort from general knowledge, not a
    /// direct copy of the official LGD gazette - spot-check before relying on it for anything
    /// beyond a candidate-facing dropdown. IDs are fixed literals (not auto-increment) so District
    /// and Thana rows can reference their parent by a stable, human-assigned key.
    /// </summary>
    public static class BangladeshGeoSeedData
    {
        public record DivisionSeed(long Id, string Name);
        public record DistrictSeed(long Id, long DivisionId, string Name);
        public record ThanaSeed(long Id, long DistrictId, string Name);

        public static readonly DivisionSeed[] Divisions =
        {
            new(1, "Barisal"),
            new(2, "Chattogram"),
            new(3, "Dhaka"),
            new(4, "Khulna"),
            new(5, "Mymensingh"),
            new(6, "Rajshahi"),
            new(7, "Rangpur"),
            new(8, "Sylhet"),
        };

        public static readonly DistrictSeed[] Districts =
        {
            // Barisal (1)
            new(1, 1, "Barisal"), new(2, 1, "Barguna"), new(3, 1, "Bhola"), new(4, 1, "Jhalokati"),
            new(5, 1, "Patuakhali"), new(6, 1, "Pirojpur"),
            // Chattogram (2)
            new(7, 2, "Bandarban"), new(8, 2, "Brahmanbaria"), new(9, 2, "Chandpur"), new(10, 2, "Chattogram"),
            new(11, 2, "Cumilla"), new(12, 2, "Cox's Bazar"), new(13, 2, "Feni"), new(14, 2, "Khagrachhari"),
            new(15, 2, "Lakshmipur"), new(16, 2, "Noakhali"), new(17, 2, "Rangamati"),
            // Dhaka (3)
            new(18, 3, "Dhaka"), new(19, 3, "Faridpur"), new(20, 3, "Gazipur"), new(21, 3, "Gopalganj"),
            new(22, 3, "Kishoreganj"), new(23, 3, "Madaripur"), new(24, 3, "Manikganj"), new(25, 3, "Munshiganj"),
            new(26, 3, "Narayanganj"), new(27, 3, "Narsingdi"), new(28, 3, "Rajbari"), new(29, 3, "Shariatpur"),
            new(30, 3, "Tangail"),
            // Khulna (4)
            new(31, 4, "Bagerhat"), new(32, 4, "Chuadanga"), new(33, 4, "Jashore"), new(34, 4, "Jhenaidah"),
            new(35, 4, "Khulna"), new(36, 4, "Kushtia"), new(37, 4, "Magura"), new(38, 4, "Meherpur"),
            new(39, 4, "Narail"), new(40, 4, "Satkhira"),
            // Mymensingh (5)
            new(41, 5, "Jamalpur"), new(42, 5, "Mymensingh"), new(43, 5, "Netrokona"), new(44, 5, "Sherpur"),
            // Rajshahi (6)
            new(45, 6, "Bogura"), new(46, 6, "Chapainawabganj"), new(47, 6, "Joypurhat"), new(48, 6, "Naogaon"),
            new(49, 6, "Natore"), new(50, 6, "Pabna"), new(51, 6, "Rajshahi"), new(52, 6, "Sirajganj"),
            // Rangpur (7)
            new(53, 7, "Dinajpur"), new(54, 7, "Gaibandha"), new(55, 7, "Kurigram"), new(56, 7, "Lalmonirhat"),
            new(57, 7, "Nilphamari"), new(58, 7, "Panchagarh"), new(59, 7, "Rangpur"), new(60, 7, "Thakurgaon"),
            // Sylhet (8)
            new(61, 8, "Habiganj"), new(62, 8, "Moulvibazar"), new(63, 8, "Sunamganj"), new(64, 8, "Sylhet"),
        };

        public static readonly ThanaSeed[] Thanas = BuildThanas();

        private static ThanaSeed[] BuildThanas()
        {
            // Ordered (not a Dictionary) so seed row IDs stay identical across every build - EF's
            // migration diffing depends on HasData producing the exact same rows every time.
            var byDistrict = new List<(long DistrictId, string[] Names)>
            {
                (1, new[] { "Barisal Sadar", "Bakerganj", "Babuganj", "Banaripara", "Gaurnadi", "Agailjhara", "Mehendiganj", "Muladi", "Wazirpur" }),
                (2, new[] { "Barguna Sadar", "Amtali", "Bamna", "Betagi", "Patharghata", "Taltali" }),
                (3, new[] { "Bhola Sadar", "Burhanuddin", "Char Fasson", "Daulatkhan", "Lalmohan", "Manpura", "Tazumuddin" }),
                (4, new[] { "Jhalokati Sadar", "Kathalia", "Nalchity", "Rajapur" }),
                (5, new[] { "Patuakhali Sadar", "Bauphal", "Dashmina", "Dumki", "Galachipa", "Kalapara", "Mirzaganj", "Rangabali" }),
                (6, new[] { "Pirojpur Sadar", "Bhandaria", "Kawkhali", "Mathbaria", "Nazirpur", "Nesarabad", "Zianagar" }),

                (7, new[] { "Bandarban Sadar", "Alikadam", "Lama", "Naikhongchhari", "Rowangchhari", "Ruma", "Thanchi" }),
                (8, new[] { "Brahmanbaria Sadar", "Akhaura", "Bancharampur", "Bijoynagar", "Kasba", "Nabinagar", "Nasirnagar", "Sarail" }),
                (9, new[] { "Chandpur Sadar", "Faridganj", "Haimchar", "Hajiganj", "Kachua", "Matlab Dakshin", "Matlab Uttar", "Shahrasti" }),
                (10, new[] { "Anwara", "Banshkhali", "Boalkhali", "Chandanaish", "Fatikchhari", "Hathazari", "Karnaphuli", "Lohagara", "Mirsharai", "Patiya", "Rangunia", "Raozan", "Sandwip", "Satkania", "Sitakunda" }),
                (11, new[] { "Cumilla Sadar", "Barura", "Brahmanpara", "Burichang", "Chandina", "Chauddagram", "Daudkandi", "Debidwar", "Homna", "Laksam", "Meghna", "Monohorgonj", "Muradnagar", "Nangalkot", "Titas" }),
                (12, new[] { "Cox's Bazar Sadar", "Chakaria", "Kutubdia", "Maheshkhali", "Pekua", "Ramu", "Teknaf", "Ukhia" }),
                (13, new[] { "Feni Sadar", "Chhagalnaiya", "Daganbhuiyan", "Parshuram", "Fulgazi", "Sonagazi" }),
                (14, new[] { "Khagrachhari Sadar", "Dighinala", "Lakshmichhari", "Mahalchhari", "Manikchhari", "Matiranga", "Panchhari", "Ramgarh" }),
                (15, new[] { "Lakshmipur Sadar", "Kamalnagar", "Raipur", "Ramganj", "Ramgati" }),
                (16, new[] { "Noakhali Sadar", "Begumganj", "Chatkhil", "Companiganj", "Hatiya", "Kabirhat", "Senbagh", "Sonaimuri", "Subarnachar" }),
                (17, new[] { "Rangamati Sadar", "Bagaichhari", "Barkal", "Belaichhari", "Juraichhari", "Kaptai", "Kawkhali", "Langadu", "Naniarchar", "Rajasthali" }),

                (18, new[] { "Dhamrai", "Savar", "Keraniganj", "Nawabganj", "Dohar" }),
                (19, new[] { "Faridpur Sadar", "Alfadanga", "Bhanga", "Boalmari", "Charbhadrasan", "Madhukhali", "Nagarkanda", "Sadarpur", "Saltha" }),
                (20, new[] { "Gazipur Sadar", "Kaliakair", "Kaliganj", "Kapasia", "Sreepur" }),
                (21, new[] { "Gopalganj Sadar", "Kashiani", "Kotalipara", "Muksudpur", "Tungipara" }),
                (22, new[] { "Kishoreganj Sadar", "Austagram", "Bajitpur", "Bhairab", "Hossainpur", "Itna", "Karimganj", "Katiadi", "Kuliarchar", "Mithamain", "Nikli", "Pakundia", "Tarail" }),
                (23, new[] { "Madaripur Sadar", "Kalkini", "Rajoir", "Shibchar" }),
                (24, new[] { "Manikganj Sadar", "Daulatpur", "Ghior", "Harirampur", "Saturia", "Shibalaya", "Singair" }),
                (25, new[] { "Munshiganj Sadar", "Gazaria", "Lohajang", "Sirajdikhan", "Sreenagar", "Tongibari" }),
                (26, new[] { "Narayanganj Sadar", "Araihazar", "Bandar", "Rupganj", "Sonargaon" }),
                (27, new[] { "Narsingdi Sadar", "Belabo", "Monohardi", "Palash", "Raipura", "Shibpur" }),
                (28, new[] { "Rajbari Sadar", "Baliakandi", "Goalandaghat", "Pangsha", "Kalukhali" }),
                (29, new[] { "Shariatpur Sadar", "Bhedarganj", "Damudya", "Gosairhat", "Naria", "Zajira" }),
                (30, new[] { "Tangail Sadar", "Basail", "Bhuapur", "Delduar", "Dhanbari", "Ghatail", "Gopalpur", "Kalihati", "Madhupur", "Mirzapur", "Nagarpur", "Sakhipur" }),

                (31, new[] { "Bagerhat Sadar", "Chitalmari", "Fakirhat", "Kachua", "Mollahat", "Mongla", "Morrelganj", "Rampal", "Sarankhola" }),
                (32, new[] { "Chuadanga Sadar", "Alamdanga", "Damurhuda", "Jibannagar" }),
                (33, new[] { "Jashore Sadar", "Abhaynagar", "Bagherpara", "Chaugachha", "Jhikargachha", "Keshabpur", "Manirampur", "Sharsha" }),
                (34, new[] { "Jhenaidah Sadar", "Harinakunda", "Kaliganj", "Kotchandpur", "Maheshpur", "Shailkupa" }),
                (35, new[] { "Batiaghata", "Dacope", "Dumuria", "Dighalia", "Koyra", "Paikgachha", "Phultala", "Rupsa", "Terokhada" }),
                (36, new[] { "Kushtia Sadar", "Bheramara", "Daulatpur", "Khoksa", "Kumarkhali", "Mirpur" }),
                (37, new[] { "Magura Sadar", "Mohammadpur", "Shalikha", "Sreepur" }),
                (38, new[] { "Meherpur Sadar", "Gangni", "Mujibnagar" }),
                (39, new[] { "Narail Sadar", "Kalia", "Lohagara" }),
                (40, new[] { "Satkhira Sadar", "Assasuni", "Debhata", "Kalaroa", "Kaliganj", "Shyamnagar", "Tala" }),

                (41, new[] { "Jamalpur Sadar", "Bakshiganj", "Dewanganj", "Islampur", "Madarganj", "Melandaha", "Sarishabari" }),
                (42, new[] { "Mymensingh Sadar", "Bhaluka", "Dhobaura", "Fulbaria", "Gaffargaon", "Gauripur", "Haluaghat", "Ishwarganj", "Muktagachha", "Nandail", "Phulpur", "Trishal" }),
                (43, new[] { "Netrokona Sadar", "Atpara", "Barhatta", "Durgapur", "Kalmakanda", "Kendua", "Khaliajuri", "Madan", "Mohanganj", "Purbadhala" }),
                (44, new[] { "Sherpur Sadar", "Jhenaigati", "Nakla", "Nalitabari", "Sreebardi" }),

                (45, new[] { "Bogura Sadar", "Adamdighi", "Dhunat", "Dhupchanchia", "Gabtali", "Kahaloo", "Nandigram", "Sariakandi", "Shajahanpur", "Sherpur", "Shibganj", "Sonatala" }),
                (46, new[] { "Chapainawabganj Sadar", "Bholahat", "Gomastapur", "Nachole", "Shibganj" }),
                (47, new[] { "Joypurhat Sadar", "Akkelpur", "Kalai", "Khetlal", "Panchbibi" }),
                (48, new[] { "Naogaon Sadar", "Atrai", "Badalgachhi", "Dhamoirhat", "Manda", "Mahadebpur", "Niamatpur", "Patnitala", "Porsha", "Raninagar", "Sapahar" }),
                (49, new[] { "Natore Sadar", "Bagatipara", "Baraigram", "Gurudaspur", "Lalpur", "Singra" }),
                (50, new[] { "Pabna Sadar", "Atgharia", "Bera", "Bhangura", "Chatmohar", "Faridpur", "Ishwardi", "Santhia", "Sujanagar" }),
                (51, new[] { "Bagha", "Bagmara", "Charghat", "Durgapur", "Godagari", "Mohonpur", "Paba", "Puthia", "Tanore" }),
                (52, new[] { "Sirajganj Sadar", "Belkuchi", "Chauhali", "Kamarkhanda", "Kazipur", "Raiganj", "Shahjadpur", "Tarash", "Ullapara" }),

                (53, new[] { "Dinajpur Sadar", "Birampur", "Birganj", "Biral", "Bochaganj", "Chirirbandar", "Fulbari", "Ghoraghat", "Hakimpur", "Kaharole", "Khansama", "Nawabganj", "Parbatipur" }),
                (54, new[] { "Gaibandha Sadar", "Fulchhari", "Gobindaganj", "Palashbari", "Sadullapur", "Saghata", "Sundarganj" }),
                (55, new[] { "Kurigram Sadar", "Bhurungamari", "Char Rajibpur", "Chilmari", "Nageshwari", "Phulbari", "Rajarhat", "Raomari", "Ulipur" }),
                (56, new[] { "Lalmonirhat Sadar", "Aditmari", "Hatibandha", "Kaliganj", "Patgram" }),
                (57, new[] { "Nilphamari Sadar", "Dimla", "Domar", "Jaldhaka", "Kishoreganj", "Saidpur" }),
                (58, new[] { "Panchagarh Sadar", "Atwari", "Boda", "Debiganj", "Tetulia" }),
                (59, new[] { "Rangpur Sadar", "Badarganj", "Gangachara", "Kaunia", "Mithapukur", "Pirgachha", "Pirganj", "Taraganj" }),
                (60, new[] { "Thakurgaon Sadar", "Baliadangi", "Haripur", "Pirganj", "Ranisankail" }),

                (61, new[] { "Habiganj Sadar", "Ajmiriganj", "Bahubal", "Baniyachong", "Chunarughat", "Lakhai", "Madhabpur", "Nabiganj", "Sayestaganj" }),
                (62, new[] { "Moulvibazar Sadar", "Barlekha", "Juri", "Kamalganj", "Kulaura", "Rajnagar", "Sreemangal" }),
                (63, new[] { "Sunamganj Sadar", "Bishwamvarpur", "Chhatak", "Derai", "Dharampasha", "Dowarabazar", "Jagannathpur", "Jamalganj", "Sullah", "Tahirpur", "South Sunamganj" }),
                (64, new[] { "Sylhet Sadar", "Balaganj", "Beanibazar", "Bishwanath", "Companiganj", "Fenchuganj", "Golapganj", "Gowainghat", "Jaintiapur", "Kanaighat", "Osmani Nagar", "Zakiganj" }),

                // Divisional-HQ districts (18 Dhaka, 10 Chattogram, 35 Khulna, 51 Rajshahi, 64 Sylhet,
                // 1 Barisal, 59 Rangpur) each originally listed only their outlying upazilas, leaving
                // out the city itself - split by its own Metropolitan Police thanas rather than an
                // upazila, and the area most candidates actually pick. Appended as second entries per
                // district (instead of folded into the first) so existing sequential Thana IDs above
                // stay stable. Mymensingh (42) has no separate metropolitan layer, so it's untouched.
                (18, new[] { "Adabor", "Badda", "Bangshal", "Bhashantek", "Bimanbandar", "Cantonment", "Chackbazar", "Dakshinkhan", "Darus Salam", "Demra", "Dhanmondi", "Gandaria", "Gulshan", "Hatirjheel", "Hazaribagh", "Jatrabari", "Kadamtali", "Kafrul", "Kalabagan", "Kamrangirchar", "Khilgaon", "Khilkhet", "Kotwali", "Lalbagh", "Mirpur", "Mohammadpur", "Motijheel", "Mugda", "New Market", "Pallabi", "Paltan", "Ramna", "Rampura", "Rupnagar", "Sabujbagh", "Shah Ali", "Shahbagh", "Shahjahanpur", "Sher-e-Bangla Nagar", "Shyampur", "Sutrapur", "Tejgaon", "Tejgaon Industrial Area", "Turag", "Uttara East", "Uttara West", "Uttarkhan", "Vatara", "Wari" }),
                (10, new[] { "Akbar Shah", "Bakalia", "Bandar", "Bayazid Bostami", "Chandgaon", "Chawkbazar", "Double Mooring", "EPZ", "Halishahar", "Khulshi", "Kotwali", "Pahartali", "Panchlaish", "Patenga", "Sadarghat" }),
                (35, new[] { "Khulna Airport", "Khalishpur", "Khan Jahan Ali", "Kotwali", "Sonadanga", "Daulatpur", "Harintana", "Lobonchora" }),
                (51, new[] { "Boalia", "Rajpara", "Shah Makhdum", "Matihar", "Chandrima", "Kashiadanga", "Katakhali", "Rajshahi Airport" }),
                (64, new[] { "Kotwali", "Jalalabad", "Sylhet Airport", "South Surma", "Shah Poran", "Moglabazar" }),
                (1, new[] { "Kotwali", "Barisal Airport", "Bandar", "Kaunia" }),
                (59, new[] { "Kotwali", "Mahiganj", "Tajhat", "Haragach" }),
            };

            var result = new List<ThanaSeed>();
            long id = 1;
            foreach (var (districtId, names) in byDistrict)
            {
                foreach (var name in names)
                {
                    result.Add(new ThanaSeed(id, districtId, name));
                    id++;
                }
            }
            return result.ToArray();
        }
    }
}
