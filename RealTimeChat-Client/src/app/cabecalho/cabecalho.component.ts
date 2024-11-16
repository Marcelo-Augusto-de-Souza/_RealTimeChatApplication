import { Component } from '@angular/core';
import { Contact } from '../user.service';
import { UserService } from '../user.service';
import { Observable, ObservedValueOf } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cabecalho',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cabecalho.component.html',
  styleUrl: './cabecalho.component.css'
})
export class CabecalhoComponent {
  activeContact$!: Observable<Contact | null>;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.activeContact$ = this.userService.activeContact$;
  }
}
