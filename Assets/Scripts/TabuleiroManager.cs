
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    [Header("Tamanho do tabuleiro")]
    public int linhas = 6;
    public int colunas = 6;

    [Header("Peças")]
    public GameObject[] prefabsPecas;
    public Transform paiDoTabuleiro;

    private GameObject[,] pecas;
    private int[,] tipos;

    private bool aProcessar = false;
    private GridLayoutGroup grelha;

    void Start()
    {
        if (prefabsPecas == null || prefabsPecas.Length < 3)
        {
            Debug.LogError("PIMBA CRUSH: São necessárias pelo menos 3 peças diferentes!");
            return;
        }

        if (paiDoTabuleiro == null)
        {
            Debug.LogError("PIMBA CRUSH: Falta ligar o Pai Do Tabuleiro!");
            return;
        }

        grelha = paiDoTabuleiro.GetComponent<GridLayoutGroup>();

        if (grelha == null)
        {
            Debug.LogError("PIMBA CRUSH: Falta o Grid Layout Group!");
            return;
        }

        grelha.enabled = true;

        CriarTabuleiro();
    }

    // CRIAR O TABULEIRO INICIAL
    void CriarTabuleiro()
    {
        pecas = new GameObject[linhas, colunas];
        tipos = new int[linhas, colunas];

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                tipos[l, c] = -1;
            }
        }

        for (int i = paiDoTabuleiro.childCount - 1; i >= 0; i--)
        {
            Transform filho = paiDoTabuleiro.GetChild(i);

            if (filho.GetComponent<Peca>() != null)
                Destroy(filho.gameObject);
        }

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                int tipo = EscolherTipoInicial(l, c);
                CriarPeca(l, c, tipo);
            }
        }

        AtualizarOrdemVisual();
    }

    // EVITAR TRIOS NO INÍCIO
    int EscolherTipoInicial(int l, int c)
    {
        List<int> disponiveis = new List<int>();

        for (int tipo = 0; tipo < prefabsPecas.Length; tipo++)
        {
            bool trioHorizontal =
                c >= 2 &&
                tipos[l, c - 1] == tipo &&
                tipos[l, c - 2] == tipo;

            bool trioVertical =
                l >= 2 &&
                tipos[l - 1, c] == tipo &&
                tipos[l - 2, c] == tipo;

            if (!trioHorizontal && !trioVertical)
                disponiveis.Add(tipo);
        }

        return disponiveis[Random.Range(0, disponiveis.Count)];
    }

    // CRIAR UMA PEÇA
    void CriarPeca(int l, int c, int tipo)
    {
        GameObject nova = Instantiate(
            prefabsPecas[tipo],
            paiDoTabuleiro,
            false
        );

        Peca componente = nova.GetComponent<Peca>();

        if (componente == null)
            componente = nova.AddComponent<Peca>();

        componente.Configurar(this, l, c);

        pecas[l, c] = nova;
        tipos[l, c] = tipo;

        StartCoroutine(componente.FadeIn(0.25f));
    }

    // ATUALIZAR A ORDEM VISUAL DA GRELHA
    void AtualizarOrdemVisual()
    {
        if (pecas == null)
            return;

        int indice = 0;

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                if (pecas[l, c] != null)
                {
                    pecas[l, c].transform.SetSiblingIndex(indice);
                }

                indice++;
            }
        }

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            paiDoTabuleiro as RectTransform
        );
    }

    public void ClicarNaPeca(Peca peca)
    {
        // Reservado para seleção de peças.
    }

    // DETETAR ARRASTAMENTO
    public void ArrastarPeca(
        Peca peca,
        int direcaoLinha,
        int direcaoColuna
    )
    {
        if (aProcessar || peca == null)
            return;

        int novaLinha = peca.linha + direcaoLinha;
        int novaColuna = peca.coluna + direcaoColuna;

        if (novaLinha < 0 || novaLinha >= linhas ||
            novaColuna < 0 || novaColuna >= colunas)
            return;

        if (Mathf.Abs(direcaoLinha) +
            Mathf.Abs(direcaoColuna) != 1)
            return;

        StartCoroutine(
            TrocarEProcessar(peca, novaLinha, novaColuna)
        );
    }

    // TROCAR E VERIFICAR COMBINAÇÕES
    IEnumerator TrocarEProcessar(
        Peca peca,
        int novaLinha,
        int novaColuna
    )
    {
        aProcessar = true;

        GameObject objetoOutra = pecas[novaLinha, novaColuna];

        if (objetoOutra == null)
        {
            aProcessar = false;
            yield break;
        }

        Peca outra = objetoOutra.GetComponent<Peca>();

        if (outra == null)
        {
            aProcessar = false;
            yield break;
        }

        TrocarDados(peca, outra);
        AtualizarOrdemVisual();

        yield return null;

        List<Peca> combinacoes = EncontrarCombinacoes();

        // SE NÃO HOUVER COMBINAÇÃO, VOLTAR ATRÁS
        if (combinacoes.Count == 0)
        {
            TrocarDados(peca, outra);
            AtualizarOrdemVisual();

            yield return new WaitForSeconds(0.15f);

            aProcessar = false;
            yield break;
        }

        // PROCESSAR COMBINAÇÕES E REAÇÕES EM CADEIA
        int seguranca = 0;

        while (combinacoes.Count >= 3 && seguranca < 30)
        {
            seguranca++;

            yield return StartCoroutine(
                RemoverERepor(combinacoes)
            );

            combinacoes = EncontrarCombinacoes();
        }

        AtualizarOrdemVisual();

        aProcessar = false;
    }

    // TROCAR PEÇAS E ATUALIZAR AS COORDENADAS
    void TrocarDados(Peca a, Peca b)
    {
        int linhaA = a.linha;
        int colunaA = a.coluna;

        int linhaB = b.linha;
        int colunaB = b.coluna;

        GameObject objetoA = pecas[linhaA, colunaA];
        GameObject objetoB = pecas[linhaB, colunaB];

        int tipoA = tipos[linhaA, colunaA];
        int tipoB = tipos[linhaB, colunaB];

        pecas[linhaA, colunaA] = objetoB;
        pecas[linhaB, colunaB] = objetoA;

        tipos[linhaA, colunaA] = tipoB;
        tipos[linhaB, colunaB] = tipoA;

        a.Configurar(this, linhaB, colunaB);
        b.Configurar(this, linhaA, colunaA);
    }

    // DETETAR TRIOS HORIZONTAIS E VERTICAIS
    List<Peca> EncontrarCombinacoes()
    {
        List<Peca> resultado = new List<Peca>();

        bool[,] marcadas = new bool[linhas, colunas];

        // HORIZONTAL
        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas - 2; c++)
            {
                int tipo = tipos[l, c];

                if (tipo < 0 || pecas[l, c] == null)
                    continue;

                int fim = c + 1;

                while (fim < colunas &&
                       tipos[l, fim] == tipo &&
                       pecas[l, fim] != null)
                {
                    fim++;
                }

                if (fim - c >= 3)
                {
                    for (int i = c; i < fim; i++)
                        marcadas[l, i] = true;
                }

                c = fim - 1;
            }
        }

        // VERTICAL
        for (int c = 0; c < colunas; c++)
        {
            for (int l = 0; l < linhas - 2; l++)
            {
                int tipo = tipos[l, c];

                if (tipo < 0 || pecas[l, c] == null)
                    continue;

                int fim = l + 1;

                while (fim < linhas &&
                       tipos[fim, c] == tipo &&
                       pecas[fim, c] != null)
                {
                    fim++;
                }

                if (fim - l >= 3)
                {
                    for (int i = l; i < fim; i++)
                        marcadas[i, c] = true;
                }

                l = fim - 1;
            }
        }

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                if (marcadas[l, c] && pecas[l, c] != null)
                {
                    Peca peca = pecas[l, c].GetComponent<Peca>();

                    if (peca != null)
                        resultado.Add(peca);
                }
            }
        }

        return resultado;
    }

    // REMOVER, FAZER DESCER E PREENCHER
    IEnumerator RemoverERepor(List<Peca> combinacoes)
    {
        bool[,] removidas = new bool[linhas, colunas];

        // MARCAR E ANIMAR AS PEÇAS
        foreach (Peca peca in combinacoes)
        {
            if (peca == null)
                continue;

            removidas[peca.linha, peca.coluna] = true;

            StartCoroutine(peca.FadeOut(0.20f));
        }

        yield return new WaitForSeconds(0.22f);

        // DESTRUIR AS PEÇAS MARCADAS
        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                if (removidas[l, c])
                {
                    if (pecas[l, c] != null)
                        Destroy(pecas[l, c]);

                    pecas[l, c] = null;
                    tipos[l, c] = -1;
                }
            }
        }

        yield return null;

        // FAZER AS PEÇAS DESCEREM EM CADA COLUNA
        for (int c = 0; c < colunas; c++)
        {
            int destino = linhas - 1;

            for (int l = linhas - 1; l >= 0; l--)
            {
                if (pecas[l, c] != null)
                {
                    if (destino != l)
                    {
                        pecas[destino, c] = pecas[l, c];
                        tipos[destino, c] = tipos[l, c];

                        pecas[l, c] = null;
                        tipos[l, c] = -1;
                    }

                    // CORRIGIR AS COORDENADAS DA PEÇA!
                    Peca componente =
                        pecas[destino, c].GetComponent<Peca>();

                    if (componente != null)
                        componente.Configurar(this, destino, c);

                    destino--;
                }
            }

            // PREENCHER TODOS OS ESPAÇOS VAZIOS
            for (int l = destino; l >= 0; l--)
            {
                int tipo = EscolherTipoParaReposicao(l, c);
                CriarPeca(l, c, tipo);
            }
        }

        AtualizarOrdemVisual();

        yield return new WaitForSeconds(0.25f);
    }

    // ESCOLHER PEÇAS NOVAS SEM CRIAR TRIOS AUTOMÁTICOS
    int EscolherTipoParaReposicao(int l, int c)
    {
        List<int> disponiveis = new List<int>();

        for (int tipo = 0; tipo < prefabsPecas.Length; tipo++)
        {
            bool trioHorizontal =
                c >= 2 &&
                tipos[l, c - 1] == tipo &&
                tipos[l, c - 2] == tipo;

            bool trioVertical =
                l + 2 < linhas &&
                tipos[l + 1, c] == tipo &&
                tipos[l + 2, c] == tipo;

            if (!trioHorizontal && !trioVertical)
                disponiveis.Add(tipo);
        }

        // Com pelo menos 3 tipos diferentes,
        // deverá existir sempre uma opção válida.
        if (disponiveis.Count == 0)
        {
            Debug.LogWarning(
                "PIMBA CRUSH: Não foi possível escolher uma peça sem criar uma combinação."
            );

            return Random.Range(0, prefabsPecas.Length);
        }

        return disponiveis[
            Random.Range(0, disponiveis.Count)
        ];
    }
}

