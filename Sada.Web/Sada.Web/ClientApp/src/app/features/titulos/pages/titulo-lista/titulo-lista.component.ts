import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Titulo {
  idtitulo: number;
  nometitulo: string;
  descricao: string;
  datavencimento: string; // <-- alterado de Date para string
  status: string;
}

@Component({
  selector: 'app-titulo-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './titulo-lista.component.html',
  styleUrls: ['./titulo-lista.component.scss']
})
export class TituloListaComponent {
  titulos: Titulo[] = []; 
  modalAberta = false;
  tituloEmEdicao: Titulo = this.criarTituloVazio();
  editando = false;
  dataVencimentoTexto = '';

  novo(): void {
    this.editando = false;
    this.tituloEmEdicao = this.criarTituloVazio();
    //this.tituloEmEdicao = { idtitulo: 0, nometitulo: '', descricao: '', datavencimento: '', status: '' };
    this.dataVencimentoTexto = '';
    this.modalAberta = true;
  }

  editar(titulo: Titulo): void {
    this.editando = true;
    this.tituloEmEdicao = { ...titulo };
    this.dataVencimentoTexto = titulo.datavencimento;
    this.modalAberta = true;
  }

  salvar(): void {
    this.tituloEmEdicao.datavencimento = this.dataVencimentoTexto;
    const nometitulo = this.tituloEmEdicao.nometitulo.trim();

    if (!nometitulo) return;

    if (this.editando) {
      const indice = this.titulos.findIndex(
        titulo => titulo.idtitulo === this.tituloEmEdicao.idtitulo
      );

      if (indice >= 0) {
        this.titulos[indice] = { ...this.tituloEmEdicao, nometitulo };
      }
    } else {
      this.titulos.push({
        ...this.tituloEmEdicao,
        idtitulo: this.proximoId(),
        nometitulo
      });
    }

    console.log('Título salvo:', this.tituloEmEdicao);
    console.log('Lista de títulos atualizada:', this.titulos);
    console.log('Data de vencimento:', this.dataVencimentoTexto);
    console.log('Status :', this.tituloEmEdicao.status);

    this.fecharModal();
  }

  excluir(titulo: Titulo): void { this.titulos = this.titulos.filter((item) => item.idtitulo !== titulo.idtitulo); }
  fecharModal(): void { this.modalAberta = false; }

  private criarTituloVazio(): Titulo {
    return { idtitulo: 0, nometitulo: '', descricao: '', datavencimento: '', status: '' };
  }

  private proximoId(): number { return Math.max(0, ...this.titulos.map((titulo) => titulo.idtitulo)) + 1; }
}
