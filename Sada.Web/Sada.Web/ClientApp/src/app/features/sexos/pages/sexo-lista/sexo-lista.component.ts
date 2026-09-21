import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Sexo, SexoService } from '../../sexo.service';

// interface Sexo {
//   idsexo: number;
//   descricao: string;
//   sigla: string;
// }

@Component({
  selector: 'app-sexo-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sexo-lista.component.html',
  styleUrls: ['./sexo-lista.component.scss']
})

export class SexoListaComponent implements OnInit {

  private readonly sexoService = inject(SexoService);

  sexos: Sexo[] = []; 
  carregando = false;
  mensagemErro = '';
  modalAberta = false;
  sexoEmEdicao: Sexo = this.criarSexoVazio();
  editando = false;
  
  ngOnInit(): void {
    this.carregarSexos();
  }
    
  carregarSexos(): void {
    this.carregando = true;
    this.mensagemErro = '';

    this.sexoService.listar().subscribe({
      next: (resultado) => {
        this.sexos = resultado;
        this.carregando = false;
      },
      error: (erro) => {
        console.error('Erro ao listar sexos:', erro);
        this.mensagemErro = 'Não foi possível carregar os tipos de sexo.';
        this.carregando = false;
      }
    });
  }
  
  novo(): void {
    this.editando = false;
    this.sexoEmEdicao = this.criarSexoVazio();  
    this.modalAberta = true;
  }

  editar(sexo: Sexo): void {
    this.editando = true;
    this.sexoEmEdicao = { ...sexo };
    this.modalAberta = true;
  }

  salvar(): void {
    //this.sexoEmEdicao.datavencimento = this.dataVencimentoTexto;
    const descricao = this.sexoEmEdicao.descricao.trim();
    const sigla = this.sexoEmEdicao.sigla.trim();
    
    if (!descricao) {
        console.log('Descrição deve ser preenchida !!!');
        return;
    }

    if (this.editando) {
      const indice = this.sexos.findIndex(
        sexo => sexo.idsexo === this.sexoEmEdicao.idsexo
      );

      if (indice >= 0) {
        this.sexos[indice] = { ...this.sexoEmEdicao, descricao };
      }
    } else {
      this.sexos.push({
        ...this.sexoEmEdicao,
        idsexo: this.proximoId(),
        descricao
      });
    }

    console.log('Título salvo:', this.sexoEmEdicao);
    console.log('Lista de tipos de sexos atualizada:', this.sexos);
    this.fecharModal();
  }

  excluir(sexo: Sexo): void { 
    this.sexos = this.sexos.filter((item) => item.idsexo !== sexo.idsexo);
    }

  fecharModal(): void {
        this.modalAberta = false;
    }

  private criarSexoVazio(): Sexo {
    return { idsexo: 0, descricao: '', sigla: ''};
  }

  private proximoId(): number {
        return Math.max(0, ...this.sexos.map((sexo) => sexo.idsexo)) + 1;
    }
}
