using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [Header("Pontuação e movimentos")]
    public TMP_Text scoreTexto;
    public TMP_Text movesTexto;
    public GameManager gameManager;

    [Header("Som das jogadas")]
    public AudioSource audioSource;
    public AudioClip somMovimento;

    private GameObject[,] pecas;
    private int[,] tipos;

    private bool aProcessar = false;
    private bool jogoTerminado = false;
    private GridLayoutGroup grelha;

    private int pontuacao = 0;
    private int movimentos = 40;

    private const int pontuacaoObjetivo = 250;

    void Start()
    {
        ReiniciarJogo();

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
        StartCoroutine(VerificarTabuleiroInicial());
    }

    // REINICIAR PONTUAÇÃO E MOVIMENTOS
    public void ReiniciarJogo()
    {
        pontuacao = 0;
        movimentos = 40;
        jogoTerminado = false;
        aProcessar = false;

        AtualizarTextos();
    }

    // ATUALIZAR OS TEXTOS
    void AtualizarTextos()
    {
        if (scoreTexto != null)
            scoreTexto.text = pontuacao.ToString();

        if (movesTexto != null)
            movesTexto.text = movimentos.ToString();
    }

    // VERIFICAR SE O JOGO TERMINOU
    void VerificarFimDoJogo()
    {
        if (jogoTerminado)
            return;

        if (pontuacao >= pontuacaoObjetivo)
        {
            jogoTerminado = true;

            if (gameManager != null)
                gameManager.MostrarVitoria();
            else
                Debug.LogError("PIMBA CRUSH: GameManager não está ligado!");
        }
        else if (movimentos <= 0)
        {
            jogoTerminado = true;

            if (gameManager != null)
                gameManager.MostrarDerrota();
            else
                Debug.LogError("PIMBA CRUSH: GameManager não está ligado!");
        }
    }

    // CRIAR O TABULEIRO
    void CriarTabuleiro()
    {
        pecas = new GameObject[linhas, colunas];
        tipos = new int[linhas, colunas];

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
                tipos[l, c] = -1;
        }

        // REMOVER PEÇAS ANTIGAS
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

        // Fade In desativado para manter as peças visíveis.
        // StartCoroutine(componente.FadeIn(0.25f));
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
                    pecas[l, c].transform.SetSiblingIndex(indice);

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
        if (aProcessar || jogoTerminado || peca == null)
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

        // JOGADA VÁLIDA: RETIRAR UM MOVIMENTO
        movimentos--;
        AtualizarTextos();

        // TOCAR SOM DA JOGADA VÁLIDA
        if (audioSource != null && somMovimento != null)
        {
            audioSource.PlayOneShot(somMovimento);
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

        // VERIFICAR SE O JOGO TERMINOU
        VerificarFimDoJogo();

        // VERIFICAR SE AINDA EXISTEM JOGADAS POSSÍVEIS
        if (!jogoTerminado && !ExisteJogadaPossivel())
        {
            yield return StartCoroutine(FazerRefresh());
        }

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

        // ATRIBUIR PONTOS À COMBINAÇÃO
        if (combinacoes.Count == 3)
            pontuacao += 5;
        else if (combinacoes.Count > 3)
            pontuacao += 10;

        AtualizarTextos();

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

                    // ATUALIZAR AS COORDENADAS DA PEÇA
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

    // VERIFICAR SE EXISTE ALGUMA TROCA QUE FORME UMA COMBINAÇÃO
    bool ExisteJogadaPossivel()
    {
        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                if (pecas[l, c] == null)
                    continue;

                // TESTAR TROCA PARA A DIREITA
                if (c + 1 < colunas && pecas[l, c + 1] != null)
                {
                    TrocarTipos(l, c, l, c + 1);

                    bool encontrou =
                        EncontrarCombinacoes().Count >= 3;

                    TrocarTipos(l, c, l, c + 1);

                    if (encontrou)
                        return true;
                }

                // TESTAR TROCA PARA BAIXO
                if (l + 1 < linhas && pecas[l + 1, c] != null)
                {
                    TrocarTipos(l, c, l + 1, c);

                    bool encontrou =
                        EncontrarCombinacoes().Count >= 3;

                    TrocarTipos(l, c, l + 1, c);

                    if (encontrou)
                        return true;
                }
            }
        }

        return false;
    }

    // TROCAR TEMPORARIAMENTE OS TIPOS PARA TESTAR UMA JOGADA
    void TrocarTipos(int l1, int c1, int l2, int c2)
    {
        int temporario = tipos[l1, c1];

        tipos[l1, c1] = tipos[l2, c2];
        tipos[l2, c2] = temporario;
    }

    // VERIFICAR O TABULEIRO QUANDO O JOGO COMEÇA
    IEnumerator VerificarTabuleiroInicial()
    {
        yield return null;

        if (!ExisteJogadaPossivel())
            yield return StartCoroutine(FazerRefresh());
    }

    // REFRESH AUTOMÁTICO
    IEnumerator FazerRefresh()
    {
        Debug.Log("PIMBA CRUSH: Sem jogadas possíveis! A fazer refresh...");

        yield return new WaitForSeconds(0.4f);

        // DESTRUIR TODAS AS PEÇAS ANTIGAS
        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                if (pecas[l, c] != null)
                    Destroy(pecas[l, c]);

                pecas[l, c] = null;
                tipos[l, c] = -1;
            }
        }

        // ESPERAR QUE O UNITY DESTRUA OS OBJETOS ANTIGOS
        yield return null;

        bool encontrouTabuleiro = false;
        int tentativas = 0;

        // CRIAR NOVOS TABULEIROS ATÉ EXISTIR UMA JOGADA
        while (!encontrouTabuleiro && tentativas < 100)
        {
            tentativas++;

            CriarTabuleiro();
            yield return null;

            encontrouTabuleiro = ExisteJogadaPossivel();
        }

        if (!encontrouTabuleiro)
        {
            Debug.LogError(
                "PIMBA CRUSH: Não foi possível criar um tabuleiro com jogadas possíveis."
            );
        }
        else
        {
            Debug.Log("PIMBA CRUSH: Novo tabuleiro pronto!");
        }

        AtualizarOrdemVisual();
    }
}


