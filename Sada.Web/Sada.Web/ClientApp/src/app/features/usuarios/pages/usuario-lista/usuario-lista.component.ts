import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface Usuario {
  idusuario: number;
  nomeusuario: string;
  nomesocial: string;
  login: string;
  senha: string;
  endereco: string;
  numeroendereco: string;
  bairro: string;
  iduf: number;
  idcidade: number;
  idsexo: number;
  email: string;
}

@Component({
  selector: 'app-usuario-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuario-lista.component.html',
  styleUrl: './usuario-lista.component.scss'
})

export class UsuarioListaComponent {
  usuarios: Usuario[] = [];
  modalAberta = false;
  usuarioEmEdicao: Usuario = this.criarUsuarioVazio();
  editando = false;

  //Limpando os atributos do usuário
  private criarUsuarioVazio(): Usuario {
    return {
      idusuario: 0,
      nomeusuario: '',
      nomesocial: '',
      login: '',
      senha: '',
      endereco: '',
      numeroendereco: '',
      bairro: '',
      iduf: 0,
      idcidade: 0,
      idsexo: 0,
      email: ''
    };
  }

  //Incremento de novo usuário
  private proximoId(): number {
    return Math.max(0, ...this.usuarios.map((usuario) => usuario.idusuario)) + 1;
  }

  fecharModal(): void { this.modalAberta = false; }

  novo(): void {
    this.editando = false;
    this.usuarioEmEdicao = this.criarUsuarioVazio();
    this.modalAberta = true;
  }

  editar(usuario: Usuario): void {
    this.editando = true;
    this.usuarioEmEdicao = { ...usuario };
    this.modalAberta = true;
  }

  salvar(): void {

    const nomeusuario = this.usuarioEmEdicao.nomeusuario.trim();
    const login = this.usuarioEmEdicao.login.trim();
    const senha = this.usuarioEmEdicao.senha.trim();
    const endereco = this.usuarioEmEdicao.endereco.trim();
    const numeroendereco = this.usuarioEmEdicao.numeroendereco.trim();
    const bairro = this.usuarioEmEdicao.bairro.trim();
    const idcidade = this.usuarioEmEdicao.idcidade;
    const iduf = this.usuarioEmEdicao.iduf;
    const idsexo = this.usuarioEmEdicao.idsexo;
    const email = this.usuarioEmEdicao.email.trim();

    if (!nomeusuario) {
      alert('O campo "Nome do Usuário" é obrigatório.');
      return;
    }

    if (!login) {
      alert('O campo "Login" é obrigatório.');
      return;
    }

    if (!senha) {
      alert('O campo "Senha" é obrigatório.');
      return;
    }

    if (!endereco) {
      alert('O campo "Endereço" é obrigatório.');
      return;
    }

    if (!numeroendereco) {
      alert('O campo "Número" é obrigatório.');
      return;
    }

    if (!bairro) {
      alert('O campo "Bairro" é obrigatório.');
      return;
    }

    if (!iduf) {
      alert('O campo "U.F." é obrigatório.');
      return;
    }

    if (!idcidade) {
      alert('O campo "Cidade" é obrigatório.');
      return;
    }

    if (!idsexo) {
      alert('O campo "Sexo" é obrigatório.');
      return;
    }

    if (!email) {
      alert('O campo "E-mail" é obrigatório.');
      return;
    }

    if (this.editando) {
      const indice = this.usuarios.findIndex(
        usuario => usuario.idusuario === this.usuarioEmEdicao.idusuario
      );

      if (indice >= 0) {
        this.usuarios[indice] = { ...this.usuarioEmEdicao, nomeusuario };
      }
    } else {
      this.usuarios.push({
        ...this.usuarioEmEdicao,
        idusuario: this.proximoId(),
        nomeusuario
      });

      console.log('Novo usuário adicionado:', this.usuarioEmEdicao);
      console.log('Login:', this.usuarioEmEdicao.login);
      console.log('Senha:', this.usuarioEmEdicao.senha);
      console.log('Endereço:', this.usuarioEmEdicao.endereco);
      console.log('Número:', this.usuarioEmEdicao.numeroendereco);
      console.log('Bairro:', this.usuarioEmEdicao.bairro);
      console.log('U.F.:', this.usuarioEmEdicao.iduf);
      console.log('Cidade:', this.usuarioEmEdicao.idcidade);
      console.log('Sexo:', this.usuarioEmEdicao.idsexo);
      console.log('E-mail:', this.usuarioEmEdicao.email);      
    }
  }
}
