import { Component, Input } from '@angular/core'

@Component({
  selector: 'app-warning',
  templateUrl: './warning.component.html',
  styleUrl: './warning.component.css',
})
export class WarningComponent {
  @Input() title: string = 'warning!'

  @Input() type: 'success' | 'attention' | 'wrong' | '' = ''

  setStyle() {
    switch (this.type) {
      case 'success':
        const success = {
          'background-color': 'var(--success-background-color)',
          color: 'var(--success-main-color)',
          border: '1px solid var(--success-main-color)',
        }
        return success

      case 'attention':
        const attention = {
          'background-color': 'var(--attention-background-color)',
          color: 'var(--attention-main-color)',
          border: '1px solid var(--attention-main-color)',
        }
        return attention

      case 'wrong':
        const wrong = {
          'background-color': 'var(--wrong-background-color)',
          color: 'var(--wrong-main-color)',
          border: '1px solid var(--wrong-main-color)',
        }
        return wrong
      default:
        const style = {
          'background-color': 'var(--focus-background-color)',
          color: 'var(--focus-main-color)',
          border: '1px solid var(--focus-main-color)',
        }
        return style
    }
  }
}
