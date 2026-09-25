import { signal, Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EMPTY, Observable, throwError } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { Dialog } from '@angular/cdk/dialog';
import { ConfirmDialogComponent } from '../confirm-dialog.component';

export interface Sexo {
  idsexo: number;
  descricao: string;
  sigla: string;
}

@Injectable({
  providedIn: 'root'
})

export class SexoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7150/sexo';
  private dialog = inject(Dialog);

    // Usando Signals para controle de estado (Padrão Angular 19)
  protected status = signal<string>('');

  listar(): Observable<Sexo[]> {
    return this.http.get<Sexo[]>(`${this.apiUrl}/listar-sexo`);
  }

  excluir(idsexo: number): Observable<void> {
    const dialogRef = this.dialog.open<boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Confirmar Exclusão',
        message: 'Tem certeza que deseja deletar?'
      }
    });

    return dialogRef.closed.pipe(
      switchMap((confirmado) => {
        if (!confirmado) {
          this.status.set('Ação cancelada pelo usuário.');
          console.log('Ação cancelada pelo usuário.');
          return EMPTY;
        }

        return this.http.delete<void>(`${this.apiUrl}/deletar-sexo/${idsexo}`);
        console.log('Item excluído com sucesso!');
      }),
      tap(() => this.status.set('Item excluído com sucesso!'))
    );
  }

  salvar(sexo: Sexo): Observable<Sexo> {
    const sexoParaSalvar: Sexo = {
      ...sexo,
      idsexo: Number(sexo.idsexo ?? 0),
      descricao: sexo.descricao?.trim().toUpperCase(),
      sigla: sexo.sigla?.trim().toUpperCase()
    };

    if (!sexoParaSalvar.descricao) {
      return throwError(() => new Error('A descrição deve ser preenchida.')
      );
    }

    if (!sexoParaSalvar.sigla) {
      return throwError(() => new Error('A sigla deve ser preenchida.')
      );
    }

    const registroExistente = sexoParaSalvar.idsexo > 0;

    if (registroExistente) {
      console.log('Alterando sexo:', sexoParaSalvar.idsexo);

      return this.http.put<Sexo>(`${this.apiUrl}/alterar-sexo`,
        sexoParaSalvar
      );
    }

    console.log('Cadastrando novo sexo');

    return this.http.post<Sexo>(`${this.apiUrl}/cadastrar-sexo`, sexoParaSalvar);
  }
}
