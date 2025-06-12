import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { IPayPalConfig, NgxPayPalModule } from 'ngx-paypal';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NgxPayPalModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected title = 'Paypal-Angular';
  public payPalConfig?: IPayPalConfig;
  private readonly apiUrl = environment.apiUrl;
  private readonly clientId = environment.clientId;
  ngOnInit(): void {
    this.initConfig();
  }

  private quantity = signal(1);
  setQuantity(quantity: number) {
    console.log(quantity);
    this.quantity.set(quantity);
  }
  initConfig(): void {
    this.payPalConfig = {
      style: {
        tagline: false,
        layout: 'horizontal',
        label: 'buynow',
      },
      clientId: this.clientId,

      createOrderOnServer: (data) =>
        fetch(this.apiUrl + '/CreatePayment', {
          method: 'post',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify([
            {
              name: 'Shoe',
              price: 62150,
              quantity: this.quantity(),
            },
          ]),
        })
          .then((res) => res.json())
          .then((order) => order.token),
      authorizeOnServer: (approveData: any) => {
        return fetch(this.apiUrl + '/ExecutePayment', {
          method: 'post',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            payerId: approveData.payerID,
            paymentId: approveData.paymentID,
          }),
        })
          .then((res) => {
            return res.json();
          })
          .then((details) => {
            alert('Authorization created for ' + details.payer_given_name);
          });
      },
      onCancel: (data, actions) => {
        console.log('OnCancel', data, actions);
      },
      onError: (err) => {
        console.log('OnError', err);
      },
      onClick: (data, actions) => {
        console.log('onClick', data, actions);
      },
    };
  }
}
