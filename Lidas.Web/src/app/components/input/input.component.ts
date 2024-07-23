import { Component, Input } from '@angular/core'

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrl: './input.component.css',
})
export class InputComponent {
  @Input() title: string = 'Reference'
  @Input() type: 'text' | 'password' = 'text'
  @Input() value: string = ''
}
