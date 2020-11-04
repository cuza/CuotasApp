import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { User } from './user';

import { environment } from '../environments/environment';

import 'rxjs/add/operator/toPromise';

@Injectable()
export class CuotaService {

  private apiUrl: string = environment.apiUrl + '/api/users';
  private headers = new HttpHeaders({'Content-Type': 'application/json'});

  constructor(private http: HttpClient) { }

  create(user: User): Promise<User> {
    return this.http.post(this.apiUrl, user, {headers: this.headers})
    .toPromise()
    .then(res => res as User)
    .catch(this.handleError);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error);
    if (error.statusText = 'Unknown Error') { error.message = 'Servidor inaccesible, revise su conexión.'; }// for demo purposes only
    return Promise.reject(error.error.error || error.message);
  }
}
