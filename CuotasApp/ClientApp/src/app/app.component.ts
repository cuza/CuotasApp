import { Component } from '@angular/core';
import { DeviceDetectorService } from 'ngx-device-detector';
import { CuotaService } from './cuota.service';
import { User } from './user';

import { MatSnackBar } from '@angular/material';

import { Observable } from 'rxjs/Rx';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [CuotaService]
})
export class AppComponent {
  title = 'app';
  user: User = { username: '', password: '', usage: 0, last_week: 0, today: 0, max: 0, year: '',
    online_time_24: '', online_time_week: '', online_time_month: '', online_time_year: ''};
  hide = true;
  showData = false;
  showLogin = true;
  porcent = '';
  color = 'primary';
  timer: Observable<number>;
  sub;

  constructor(private service: CuotaService, private deviceService: DeviceDetectorService, public snackBar: MatSnackBar) {
    this.timer = Observable.timer(60000, 60000);
    // if(this.redirect()) {
    //
    // }
  }


  getData() {
    this.service.create(this.user).then( data => {
      console.log(data);
      this.snackBar.open('La información se actualizará automáticamente cada 1 min', 'Cerrar', {
        duration: 2500
      });
      this.user.usage = data.usage;
      this.user.last_week = data.last_week;
      this.user.today = data.today;
      this.user.max = data.max;
      this.user.year = data.year;
      this.user.online_time_24 = data.online_time_24;
      this.user.online_time_week = data.online_time_week;
      this.user.online_time_month = data.online_time_month;
      this.user.online_time_year = data.online_time_year;
      this.showData = true;
      this.showLogin = false;
      this.porcent = ((data.usage / this.user.max) * 100).toFixed(2);
      if (parseFloat(this.porcent) >= 100) {
        this.color = 'warn';
      }
      this.timer.subscribe(t => {
        this.update();
      });
    }).catch(error => {
      this.snackBar.open(error, 'Cerrar', {
        duration: 4000
      });
    });
  }

  backClick() {
    this.showData = false;
    this.showLogin = true;
    this.color = 'primary';
    this.porcent = '';
  }

  update() {
    if (this.showData) {
      this.service.create(this.user).then( data => {
        this.user.usage = data.usage;
        this.user.last_week = data.last_week;
        this.user.today = data.today;
        this.user.max = data.max;
        this.porcent = ((data.usage / this.user.max) * 100).toFixed(2);
        if (parseFloat(this.porcent) >= 100) {
          this.color = 'warn';
        }
      });

    }
  }

  redirect() {
    const info = this.deviceService.getDeviceInfo();
    const browser = info.browser;
    const version = parseFloat(info.browser_version);
    if (browser === 'chrome') {
      if (version < 54 ) { return true; }
    } else if (browser === 'firefox') {
      if (version < 54 ) { return true; }
    }

    return false;
  }

  compare() {
    if (this.user.usage > this.user.max) {
      return true;
    }
    return false;
  }

}
