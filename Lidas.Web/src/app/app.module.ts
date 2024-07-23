import { NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { FormsModule } from '@angular/forms'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { MenuComponent } from './components/menu/menu.component'
import { InputComponent } from './components/input/input.component'
import { FormContainerComponent } from './components/form-container/form-container.component'
import { ButtonComponent } from './components/button/button.component'
import { WarningComponent } from './components/warning/warning.component';
import { LoginComponent } from './pages/login/login.component';
import { AdminComponent } from './pages/admin/admin.component'

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    InputComponent,
    FormContainerComponent,
    ButtonComponent,
    WarningComponent,
    LoginComponent,
    AdminComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, FormsModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
