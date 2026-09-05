import { Routes } from '@angular/router';
import { TituloListaComponent } from './features/titulos/pages/titulo-lista/titulo-lista.component';
import { UsuarioListaComponent } from './features/usuarios/pages/usuario-lista/usuario-lista.component';

export const routes: Routes = [
  { path: 'titulo', component: TituloListaComponent },
  { path: 'usuario', component: UsuarioListaComponent },
  { path: '', pathMatch: 'full', redirectTo: 'titulo' },
  { path: '', pathMatch: 'full', redirectTo: 'usuario' }
];
