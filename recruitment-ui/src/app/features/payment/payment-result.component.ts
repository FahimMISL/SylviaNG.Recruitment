import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-payment-result',
  standalone: true,
  template: `
    <div class="payment-result">
      <div class="result-card">
        @if (status() === 'success') {
          <div class="result-icon success">&#10003;</div>
          <h2>Payment Successful</h2>
          <p>Your application fee has been paid. You can now submit your application.</p>
        } @else if (status() === 'fail') {
          <div class="result-icon fail">&#10007;</div>
          <h2>Payment Failed</h2>
          <p>Your payment could not be processed. Please try again.</p>
        } @else {
          <div class="result-icon cancel">&#8635;</div>
          <h2>Payment Cancelled</h2>
          <p>You cancelled the payment. You can try again from the job listing.</p>
        }
        <button class="btn-back" (click)="goToJobs()">Back to Jobs</button>
      </div>
    </div>
  `,
  styles: [`
    .payment-result { display: flex; justify-content: center; align-items: center; min-height: 60vh; padding: 32px; }
    .result-card { text-align: center; background: white; border-radius: 12px; padding: 48px; box-shadow: 0 2px 12px rgba(0,0,0,0.08); max-width: 400px; width: 100%; }
    .result-icon { font-size: 48px; width: 80px; height: 80px; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 20px; }
    .result-icon.success { background: #d1fae5; color: #059669; }
    .result-icon.fail { background: #fee2e2; color: #dc2626; }
    .result-icon.cancel { background: #fef3c7; color: #d97706; }
    h2 { margin: 0 0 8px; }
    p { color: #666; margin: 0 0 24px; }
    .btn-back { background: #0a1628; color: white; border: none; padding: 12px 32px; border-radius: 6px; font-weight: 600; cursor: pointer; }
  `],
})
export class PaymentResultComponent implements OnInit {
  status = signal<'success' | 'fail' | 'cancel'>('success');

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    const queryStatus = this.route.snapshot.queryParamMap.get('status');
    if (queryStatus) {
      if (queryStatus === 'failed' || queryStatus === 'fail') this.status.set('fail');
      else if (queryStatus === 'cancelled' || queryStatus === 'cancel') this.status.set('cancel');
      else this.status.set('success');
    } else {
      const path = this.route.snapshot.url[this.route.snapshot.url.length - 1]?.path;
      if (path === 'fail') this.status.set('fail');
      else if (path === 'cancel') this.status.set('cancel');
      else this.status.set('success');
    }
  }

  goToJobs(): void {
    this.router.navigate(['/browse-jobs']);
  }
}
