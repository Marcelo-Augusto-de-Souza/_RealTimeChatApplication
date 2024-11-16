import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../user.service';
import { FormsModule, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-tela-inicio',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule, HttpClientModule],
  providers: [UserService],
  templateUrl: './tela-inicio.component.html',
  styleUrl: './tela-inicio.component.css'
})
export class TelaInicioComponent {
  nameControl = new FormControl('', [Validators.required]);

  constructor(private router: Router, private userService: UserService) {}

  get isNameInvalid() {
    return this.nameControl.invalid && this.nameControl.touched;
  }

  irParaChat() {
    if (this.nameControl.valid) {
      const name = this.nameControl.value ?? '';
      
      // Envia o nome ao servidor
      this.userService.setUserName(name).subscribe({
        next: () => {
          this.router.navigate(['/chat']);
        },
        error: (error) => {
          console.error('Erro ao enviar nome:', error);
        },
        complete: () => {
          console.log('Requisição concluída');
        }
      });
    }
  }
}