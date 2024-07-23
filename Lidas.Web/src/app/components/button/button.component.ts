import { Component, Input } from '@angular/core'

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  @Input() title: string = 'buttom'
  @Input() color: 'success' | 'attention' | 'wrong' | '' = ''
  @Input() setEvent: (e: MouseEvent) => void = () => {}

  background = [
    'var(--focus-main-color)',
    'var(--success-main-color)',
    'var(--attention-main-color)',
    'var(--wrong-main-color)',
  ]

  setColor() {
    switch (this.color) {
      case 'success':
        return { 'background-color': this.background[1] }

      case 'attention':
        return { 'background-color': this.background[2] }

      case 'wrong':
        return { 'background-color': this.background[3] }

      default:
        return { 'background-color': this.background[0] }
    }
  }
}
