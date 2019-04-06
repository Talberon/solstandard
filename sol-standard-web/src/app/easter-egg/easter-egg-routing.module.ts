import { AuthGuard } from './../auth/auth.guard';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SecretPageComponent } from './secret-page/secret-page.component';

const routes: Routes = [
  {
    path: 'secret',
    component: SecretPageComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EasterEggRoutingModule { }
