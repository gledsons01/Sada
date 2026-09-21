import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
  //private readonly apiUrl = 'http://localhost:5216/sexo';
  private readonly apiUrl = 'https://localhost:7150/sexo';

  listar(): Observable<Sexo[]> {
    return this.http.get<Sexo[]>(`${this.apiUrl}/listar-sexo`);
  }
}