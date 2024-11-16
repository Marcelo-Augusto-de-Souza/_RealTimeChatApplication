import { Routes } from '@angular/router';
import { TelaInicioComponent } from './tela-inicio/tela-inicio.component';
import { ChatComponent } from './chat/chat.component';

export const routes: Routes = [
    {
        path: '',
        component: TelaInicioComponent,
        title: 'Página Inicial'
    },
    {
        path: 'chat',
        component: ChatComponent,
        title: 'Minhas Conversas'
    }
];
