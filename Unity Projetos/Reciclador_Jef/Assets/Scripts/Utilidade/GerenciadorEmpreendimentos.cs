using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe para gerenciar a lista dos empreendimentos carregados.
/// </summary>
public class GerenciadorEmpreendimentos : MonoBehaviour
{
	/// <summary>	/// Dicionário com todos os empreendimentos. A chave é o identificador.	/// </summary>
	public static Dictionary<string,Empreendimento> dicionarioEmpreendimentos = 
		new Dictionary<string, Empreendimento>();

	/// <summary> 	/// Usado para mostrar os empreendimentos apenas quando estiverem carregados.	/// </summary>
	static bool _carregado = false;
	/// <summary> 	/// Usado para mostrar os empreendimentos apenas quando estiverem carregados.	/// </summary>
	public static bool carregado { get { return _carregado; } }

	/// <summary>
	/// Carrega a lista dos empreendimentos e a coloca em um dicionario
	/// </summary>
	void Awake()
	{
		Empreendimento [] listaEmpreendimentos = 	
			GerenciadorCarregamento.CarregarEmpreendimentos();

		foreach(Empreendimento e in listaEmpreendimentos)
		{
			e.Reiniciar();
			// TODO: e.Carregar();
			dicionarioEmpreendimentos.Add(e.identificador, e);
		}

		_carregado = true;
	}

	/// <summary>
	/// Verifica se a lista com os emprendimentos contem o empreendimento requisitado no nivel correto.
	/// </summary>
	/// <returns><c>true</c>, se possuir o empreendimento no nivel correto, <c>false</c> caso contrario.</returns>
	/// <param name="identificador">Identificador do empreemento requisitado.</param>
	/// <param name="nivel">Nivel requisitado.</param>
	static public bool TemRequisito(string identificador, int nivel)
	{
		if (dicionarioEmpreendimentos.ContainsKey(identificador))
		{
			if (dicionarioEmpreendimentos[identificador].nivel >= nivel)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary> 
	/// Compra o empreendimento selecionado, se este atender aos pre-requisitos.
	/// </summary>
	public void Comprar()
	{
		UI_Empreendimento.ComprarEstatico();
	}
}

