import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Sexo, SexoService } from '../../sexo.service';

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
    const sexoParaSalvar: Sexo = {
      ...this.sexoEmEdicao,
      idsexo: Number(this.sexoEmEdicao.idsexo ?? 0),
      descricao: (this.sexoEmEdicao.descricao ?? '').trim(),
      sigla: (this.sexoEmEdicao.sigla ?? '').trim()
    };

    this.carregando = true;
    this.mensagemErro = '';

    this.sexoService.salvar(sexoParaSalvar).subscribe({
      next: () => {
        this.fecharModal();
        this.carregarSexos();
      },
      error: (erro) => {
        console.error('Erro ao salvar sexo:', erro);
        this.mensagemErro = erro.error?.message ?? erro.message ??
          'Não foi possível salvar o tipo de sexo.';
        this.carregando = false;
      }
    });
  }
  excluir(sexo: Sexo): void {
    this.mensagemErro = '';

    this.sexoService.excluir(sexo.idsexo).subscribe({
      next: () => {
        this.carregarSexos();
      },
      error: (erro) => {
        console.error('Erro ao excluir sexo:', erro);
        this.mensagemErro = erro.error?.message ?? erro.message ??
          'Não foi possível excluir o tipo de sexo.';
      }
    });
  }
  fecharModal(): void {
        this.modalAberta = false;
    }

  private criarSexoVazio(): Sexo {
    return { idsexo: 0, descricao: '', sigla: ''};
  }

}
