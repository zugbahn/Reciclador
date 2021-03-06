﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuncoesEmpreendimentos : MonoBehaviour
{
	static List<IValoresEmpreendimentos> listaValoresEmpreendimento = null;

	public static Empreendimento [] CriarEmpreendimentos()
	{

		List<Empreendimento> listaEmpreendimentos = new List<Empreendimento>();
		// ******************************************************** //
		// Os empreendimentos deverão ser criados na classe ClassesDosEmpreendimentos
		// Para adicionar todos os empreendimentos é só copiar a linha abaixo, e alterar o nome da classe "EmpreendimentoExemplo", para a classe criada.
		//new EmpreendimentoExemplo();
		listaEmpreendimentos.Add(new Empreendimento(new ColetaSeletiva()));
		listaEmpreendimentos.Add(new Empreendimento(new OficinaBrinquedosReciclaveis()));
		listaEmpreendimentos.Add(new Empreendimento(new PrensaPapel()));
		listaEmpreendimentos.Add(new Empreendimento(new FabricaVitrais()));
		listaEmpreendimentos.Add(new Empreendimento(new FerroVelho()));
		listaEmpreendimentos.Add(new Empreendimento(new ViveiroGarrafasPet()));
		listaEmpreendimentos.Add(new Empreendimento(new TrituradorResiduosSolidos()));
		listaEmpreendimentos.Add(new Empreendimento(new UsinaReciclagem()));

		// EasterEggs
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggLampada()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggMadeira()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggIsopor()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggFraldas()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggLataTinta()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggPatoBorracha()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggSeringa()));
		listaEmpreendimentos.Add(new Empreendimento(new EasterEggPneu()));

		// ******************************************************** //
		// Essa parte não precisa ser alterada
		//List<Empreendimento> listaEmpreendimentos = new List<Empreendimento>();

		/*
		foreach(IValoresEmpreendimentos valor in listaValoresEmpreendimento)
		{
			Empreendimento empreendimento = new Empreendimento();
			empreendimento.valores			= valor;
			empreendimento.nome				= valor.Nome();
			empreendimento.identificador	= valor.Identificador();

			listaEmpreendimentos.Add(empreendimento);
		}
		*/

		return listaEmpreendimentos.ToArray();
	}
}

/*
public class FuncoesEmpreendimentos : MonoBehaviour
{
	static List<ValoresEmpreendimentos> listaValoresEmpreendimento = null;

	public static Empreendimento [] CriarEmpreendimentos()
	{
		// ******************************************************** //
		// Os empreendimentos deverão ser criados na classe ClassesDosEmpreendimentos
		// Para adicionar todos os empreendimentos é só copiar a linha abaixo, e alterar o nome da classe "EmpreendimentoExemplo", para a classe criada.
		//new EmpreendimentoExemplo();
		new ColetaSeletiva();
		new OficinaBrinquedosReciclaveis();
		new PrensaPapel();
		new FabricaVitrais();
		new FerroVelho();
		new ViveiroGarrafasPet();
		new TrituradorResiduosSolidos();
		new UsinaReciclagem();

		// EasterEggs
		new EasterEggLampada();
		new EasterEggMadeira();
		new EasterEggIsopor();
		new EasterEggFraldas();
		new EasterEggLataTinta();
		new EasterEggPatoBorracha();
		new EasterEggSeringa();
		new EasterEggPneu();

		// ******************************************************** //
		// Essa parte não precisa ser alterada
		List<Empreendimento> listaEmpreendimentos = new List<Empreendimento>();

		foreach(ValoresEmpreendimentos valor in listaValoresEmpreendimento)
		{
			Empreendimento empreendimento = new Empreendimento();
			empreendimento.valores			= valor;
			empreendimento.nome				= valor.nome;
			empreendimento.identificador	= valor.identificador;

			listaEmpreendimentos.Add(empreendimento);
		}

		return listaEmpreendimentos.ToArray();
	}

	public static void AdicionarValoresEmpreendimento(
		string nome,
		string identificador,

		ValoresEmpreendimentos.DelegateTaxaSeparacaoLixo taxaSeparacaoLixo,
		ValoresEmpreendimentos.DelegateDinheiroPorTempo dinheiroPorTempo,
		ValoresEmpreendimentos.DelegateAumentoXP aumentoXP,
		ValoresEmpreendimentos.DelegateNivelMinimoLixo nivelMinimoLixo,

		ValoresEmpreendimentos.DelegateLimiteRecicladoras limiteRecicladoras,
		ValoresEmpreendimentos.DelegateValorDeVenda valorDeVenda,
		ValoresEmpreendimentos.DelegateVelocidadeReciclagem velocidadeReciclagem,
		ValoresEmpreendimentos.DelegateVelocidadeAparecerLixo velocidadeAparecerLixo,

		ValoresEmpreendimentos.DelegateSeparacaoAutomatica separacaoAutomatica,
		ValoresEmpreendimentos.DelegateDescricao descricao,
		ValoresEmpreendimentos.DelegateCustos custos,
		ValoresEmpreendimentos.DelegateNivelRequisito nivelRequisito )
	{
		ValoresEmpreendimentos valoresEmpreendimento = new ValoresEmpreendimentos(
			nome,
			identificador,

			taxaSeparacaoLixo,
			dinheiroPorTempo,
			aumentoXP,
			nivelMinimoLixo,

			limiteRecicladoras,
			valorDeVenda,
			velocidadeReciclagem,
			velocidadeAparecerLixo,

			separacaoAutomatica,
			descricao,
			custos,
			nivelRequisito
		);

		if (listaValoresEmpreendimento == null)
		{
			listaValoresEmpreendimento = new List<ValoresEmpreendimentos>();
		}

		listaValoresEmpreendimento.Add(valoresEmpreendimento);
	}
}
//*/