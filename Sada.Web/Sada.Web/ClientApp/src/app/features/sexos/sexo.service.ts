import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

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

  listar(): Observable<Sexo[]> {
    return this.http.get<Sexo[]>(`${this.apiUrl}/listar-sexo`);
  }

  excluir(idsexo: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/excluir-sexo/${idsexo}`);
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

      return this.http.put<Sexo>(`${this.apiUrl}/alterar-sexo/${sexoParaSalvar.idsexo}`,
        sexoParaSalvar
      );
    }

    console.log('Cadastrando novo sexo');

    return this.http.post<Sexo>(`${this.apiUrl}/cadastrar-sexo`, sexoParaSalvar);
  }
}
