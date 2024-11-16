import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UserService, Contact } from '../user.service';
import { FormsModule, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { MessageList } from '../user.service';

@Component({
  selector: 'app-area-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './area-chat.component.html',
  styleUrl: './area-chat.component.css'
})

export class AreaChatComponent implements OnInit{
  
  nameControl = new FormControl('', [Validators.required]);
  activeContact$!: Observable<Contact | null>;
  activeMessages$!: Observable<MessageList | null>;
  mensagemConteudo: string = '';
  activeContactId: number | null = null;
  
  constructor(private userService: UserService) {}

  ngOnInit() {
    this.activeContact$ = this.userService.activeContact$;
    this.userService.startPollingMessages(100);
    this.userService.activeContactId$.subscribe(id => {
      this.activeContactId = id;
    });
  }

  get isNameInvalid() {
    return this.nameControl.invalid && this.nameControl.touched;
  }

  get filteredMessages() {
    console.log("Message List: ",this.userService.messagesList);
    return this.userService.messagesList.messages;
  }
  
  sendMessage() {
    console.log(this.userService.messagesList);
    this.userService.currentClientId$.subscribe(senderId => {
      this.activeContact$.subscribe(contact => {
        if (contact && this.mensagemConteudo.trim()) {
          // Envia a mensagem com os valores corretos de senderId e contact.clientId como receiverId
          this.userService.sendMessage(contact.clientId, this.mensagemConteudo).subscribe(() => {
            this.mensagemConteudo = '';
          });
        }
      });
    });
  } 

}