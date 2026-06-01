# Sada

A aplicação se basea no seguinte modo:

1 - Cadastro
Post 'https://localhost:7150/titulo/cadastrar'
Parâmetros: FromBody;
Exemplo: {
		"titulo": "Cadastro de Titulo - 00",
		"descricao": "Cadastro titulo 00 pela versão 00",
		"vencimento": "2026-05-30T05:20:09.546Z",
		"status": "P"
		}
-----------------------------------------------------------
2 - Listar
Get: 'https://localhost:7150/titulo/listar-todos-titulos'
Parâmetros: Não se aplica;
Exemplo: 
-----------------------------------------------------------
3 - Listar
Get: 'titulo/listar-titutlos-status-vencimento'
Parâmetros: FromBody;
Exemplo: {
  "Vencimento": "2014-08-09",
  "Status" : "C"
}

4 - Alterar
Put: 'https://localhost:7150/titulo/alterar-titulo'
Parâmetros: {
    "titulo": "Cadastro de Titulo - 00",
    "descricao": "Cadastro titulo 00 pela versão 00",
    "vencimento": "2026-05-31T05:20:09.546Z",
    "status": "C"
 }
 
 5 - Exclusão
 Delete: 'https://localhost:7150/titulo-apagar-titulo'
 Parâmetros: {
    "descricao": "Cadastro titulo 00 pela versão 00",
    "vencimento": "2026-05-31T05:20:09.546Z",
    "status": "C"
 }
 
 
 
