using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjAreaReciclavel : MonoBehaviour
{
	public GameObject 	objPontuacao;

	public Image		imgBarraCarregamento;
	public Text			txtLimiteListaReciclagem;

	public Reciclavel.Tipo tipo = Reciclavel.Tipo.Papel;

	public float	duracaoReciclagem		= 5;
	public int		tamanhoListaReciclagem	= 5;

	public long		pontuacaoBasica			= 5;

	public int 		xpAoMandarReciclar		= 2;

	[HideInInspector]
	public Vector2 area;
	
	RectTransform areaLixeira;

	List<ObjReciclavel> listaReciclando = new List<ObjReciclavel>();

	float proximoTempoReciclagem = 0;
	float ultimaDuracao = 0;

	void Awake()
	{
		if (areaLixeira == null)
		{
			areaLixeira = transform.FindChild("_Area").
				GetComponent<RectTransform>();
		}

		area = areaLixeira.sizeDelta;
		ultimaDuracao = duracaoReciclagem;
	}

	void Update()
	{
		VerificarReciclando();
	}

	void VerificarReciclando()
	{
		imgBarraCarregamento.fillAmount = 0;
		if (listaReciclando.Count > 0)
		{
			if (Time.time > proximoTempoReciclagem)
			{
				Pronto();
				if (listaReciclando.Count > 0)
				{
					proximoTempoReciclagem = 
						Time.time + DuracaoReciclagemAtual();
				}
			}
			else
			{
				imgBarraCarregamento.fillAmount = 
					(proximoTempoReciclagem - Time.time) / 
						ultimaDuracao;
			}
		}
		txtLimiteListaReciclagem.text = "" +
			listaReciclando.Count + "/" + TamanhoLista();
	}

	float DuracaoReciclagemAtual()
	{
		ultimaDuracao = ObjEmpreendimentos.
			TempoReciclar(duracaoReciclagem, (int) tipo);

		return ultimaDuracao;
	}

	void Pronto()
	{
		long pontos = 
			ObjEmpreendimentos.
				ValorVendaAjeitado(pontuacaoBasica, (int) tipo);

		Jogador.Pontuar(pontos);

		Instantiate<GameObject>(objPontuacao).
			GetComponent<ObjTextoFlutuante>().Criar(
				"$"+pontos, transform.position);

		listaReciclando.RemoveAt(0);
	}

	int TamanhoLista()
	{
		return ObjEmpreendimentos.LimiteLixeira(
			tamanhoListaReciclagem, (int)tipo);
	}

	public bool Reciclar(ObjReciclavel reciclavel)
	{
		if (listaReciclando.Count >= TamanhoLista())
		{
			return false;
		}

		reciclavel.Reciclar(transform, ultimaDuracao);
		listaReciclando.Add(reciclavel);

		ObjGerenciadorLixo.CriarXP(
			xpAoMandarReciclar, reciclavel.transform);

		if (listaReciclando.Count == 1)
		{
			proximoTempoReciclagem = Time.time + DuracaoReciclagemAtual();
		}
		return true;
	}
}

