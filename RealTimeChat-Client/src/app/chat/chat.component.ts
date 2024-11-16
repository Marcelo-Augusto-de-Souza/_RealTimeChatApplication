import { Component } from '@angular/core';
import { SideBarComponent } from '../side-bar/side-bar.component';
import { CabecalhoComponent } from '../cabecalho/cabecalho.component';
import { AreaChatComponent } from '../area-chat/area-chat.component';
import { Contact, UserService } from '../user.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [SideBarComponent, CabecalhoComponent, AreaChatComponent, CommonModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {
  activeContact$: Observable<Contact | null>;
  
  constructor(private userService: UserService) {
    this.activeContact$ = this.userService.activeContact$;
  }

  onContactSelected(contact: Contact) {
    const currentUserId = this.userService.getCurrentClientId();
    if (currentUserId) {
      this.userService.startConversation(currentUserId, contact.clientId)
        .subscribe({
          next: () => {
            console.log('Conversação iniciada com sucesso');
            this.userService.setActiveContact(contact);
          },
          error: (error) => {
            console.error('Erro ao iniciar conversação:', error);
          }
        });
    }
  }
}
