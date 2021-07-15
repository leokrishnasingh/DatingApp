import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};
  currentUser$ :Observable<User>;
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
    this.currentUser$= this.accountService.currentUser$;
  }

  login(){
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    },error => {console.log(error)} );
    ;
  }

  logout(){
    this.accountService.logout();
  }

  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe(user => {
  //     if(user!=null) this.loggedIn=true;
  //   }, error =>{
  //     console.log(error);
  //   })
  // }
}
