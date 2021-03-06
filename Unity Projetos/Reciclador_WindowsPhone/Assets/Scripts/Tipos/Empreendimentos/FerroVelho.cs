﻿using UnityEngine;
using System.Collections;

public class FerroVelho : IValoresEmpreendimentos
{
	
	public int ID()
	{
		return -1;
	}
	string 	nome			= "Ferro Velho";
	string 	identificador	= "FerroVelho";

	public string Nome()
	{
		return nome;
	}

	public string Identificador()
	{
		return identificador;
	}

	public int		TaxaSeparacaoLixo(int nivel)
	{
		int retorno = nivel / 7;
		return retorno;
	}

	public long		DinheiroPorTempo(int nivel)
	{
		int v = nivel - 0;
		if (v <= 0) return 0;
		long retorno = v * 5;
		return retorno;
	}

	public float	AumentoXP(int nivel)
	{
		// 5% de XP por nível
		float retorno = nivel * 0.09f;
		return retorno;
	}

	// Não altera nada
	public int 		NivelMinimoLixo(int nivel)
	{
		int retorno = 0;
		return retorno;
	}

	//
	public int []	LimiteRecicladoras(int nivel)
	{
		// Papel, Vidro, Metal, Plástico
		int [] retorno = {0,0,0,0};
		int div = 2;
		retorno[2] = (nivel + 1) / div + 1;
		return retorno;
	}

	int redutorNivelValorVenda = 3;
	public float []	ValorDeVenda(int nivel)
	{
		int nv = nivel - redutorNivelValorVenda;
		float [] retorno = {0f,0f,0f,0f};
		if (nv <= 0) return retorno;

		float porcentagem = 1f;
		retorno[2] = ((nv * (nv + 1)) / 2) * porcentagem;

		return retorno;
	}

	public float []	VelocidadeReciclagem(int nivel)
	{
		float [] retorno = {0f,0f,0f,0f};
		retorno[2] = nivel * 0.05f;
		return retorno;
	}
	//

	// NÃO IMPLEMENTADO, pode deixar como zero mesmo
	public float		VelocidadeAparecerLixo(int nivel)
	{
		float retorno = 0;
		return retorno;
	}

	// NÃO IMPLEMENTADO, pode deixar como zero mesmo
	public int 		SeparacaoAutomatica(int nivel)
	{
		// Esse empreendimento não mexe com esse atributo
		int retorno = 0;
		return retorno;
	}

	public string 		Descricao(int nivel)
	{
		// Descrição pode chamar outras funções, para mostrar valores exatos
		string retorno = "";
		//retorno += "Custo: "+Custos(nivel)+"\n\n";
		if (TaxaSeparacaoLixo(nivel+1) > 0)
		{
			retorno += "Dano extra:\t\t"+TaxaSeparacaoLixo(nivel)+" -> "+ TaxaSeparacaoLixo(nivel+1)+"\n";
		}
		retorno += "$ por tempo:\t"+DinheiroPorTempo(nivel)+" -> "+ DinheiroPorTempo(nivel+1)+"\n";
		retorno += "XP extra:\t\t\t"+(AumentoXP(nivel)*100f).ToString("0")+"% -> "+(AumentoXP(nivel+1)*100f).ToString("0")+"%\n";
		retorno += "$ reciclagem Metal:\t"+(ValorDeVenda(nivel)[2]*100f).ToString("0")+"% -> "+(ValorDeVenda(nivel+1)[2]*100f).ToString("0")+"%\n";
		retorno += "Limite Recic Metal:\t"+(LimiteRecicladoras(nivel)[2])+" -> "+(LimiteRecicladoras(nivel+1)[2])+"\n";
		retorno += "Vel Reciclagem Mtl:\t"+(VelocidadeReciclagem(nivel)[2]*100f).ToString("0")+"% -> "+(VelocidadeReciclagem(nivel+1)[2]*100f).ToString("0")+"%\n";

		return retorno;
	}

	public string DescricaoTexto(int nivel)
	{
		string retorno = "O benefício da reciclagem e do reaproveitamento de sucatas de metal reduz o consumo de energia para a produção do alumínio e outros metais, preserva o meio ambiente e movimenta a economia, gerando empregos e fonte de renda.";
		return retorno;
	}

	// REQUISITOS
	public long		Custos(int nivel)
	{
		int nv = nivel + 1;
		long retorno = nv * nv * 50; // nível ao quadrado * 10
		return retorno;
	}

	public int		NivelRequisito(int nivel)
	{
		int retorno = nivel * 4 + 11;
		return retorno;
	}
}

