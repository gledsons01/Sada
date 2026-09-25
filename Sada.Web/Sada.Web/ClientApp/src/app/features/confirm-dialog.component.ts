import { Component, inject } from '@angular/core';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  template: `
    <div class="dialog-container">
      <h3>{{ data.title }}</h3>
      <p>{{ data.message }}</p>
      
      <div class="dialog-actions">
        <button (click)="cancelar()">Não</button>
        <button (click)="confirmar()" class="btn-confirm">Sim</button>
      </div>
    </div>
  `,
  styles: `
    .dialog-container { padding: 20px; background: white; border-radius: 8px; min-width: 300px; }
    .dialog-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
    .btn-confirm { background: #d32f2f; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; }
  `
})
export class ConfirmDialogComponent {
  // Injeção de dependência moderna do Angular
  private dialogRef = inject(DialogRef<boolean>);
  protected data = inject<{ title: string; message: string }>(DIALOG_DATA);

  confirmar() {
    this.dialogRef.close(true);
  }

  cancelar() {
    this.dialogRef.close(false);
  }
}