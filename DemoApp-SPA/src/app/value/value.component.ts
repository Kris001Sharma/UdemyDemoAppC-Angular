import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

  values: any;
  // use this values inside our template, which is populated with the response

  // Dependency Injection of httpClient
  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  getValues(){
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    });
    /**subscribe(next?: (value: Object) => void, error? (error: any) => void, complete?: ()=> void): Subscription
     * next : what to do when we receive the observable, 
     * this takes a function taking Object received from server ie. our response
     */
  }

}
