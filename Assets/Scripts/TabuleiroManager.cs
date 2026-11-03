using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    [Header("Tabuleiro")]
    public int linhas = 6;
    public int colunas = 6;
    public float tamanhoPeca = 100f;

    [Header("Prefabs das Peças")]
    public GameObject[] prefabsPecas;

    [Header("Área do Tabuleiro")]
    public RectTransform areaTabuleiro;

    private GameObject[,] pecas;
    private int[,] tipos;

    private Peca pecaSelecionada;
    private bool aProcessar = false;

    // SEGURANÇA:
    // nunca permite mais de 10 cascatas seguidas
    private const int MAX_CASCATAS = 10;

    void Start()
    {
        CriarTabuleiroInicial();
    }

    void OnDisable()
    {
        aProcessar = false;
        pecaSelecionada = null;
    }

    // =========================================================
    // TABULEIRO INICIAL
    // =========================================================

    void CriarTabuleiroInicial()
    {
        pecas = new GameObject[linhas, colunas];
        tipos = new int[linhas, colunas];

        for (int linha = 0; linha < linhas; linha++)
        {
            for (int coluna = 0; coluna < colunas; coluna++)
            {
                int tipo;

                do
                {
                    tipo = Random.Range(
                        0,
                        prefabsPecas.Length
                    );
                }
                while (
                    CriariaCombinacaoInicial(
                        linha,
                        coluna,
                        tipo
                    )
                );

                tipos[linha, coluna] = tipo;

                CriarPeca(
                    linha,
                    coluna,
                    tipo
                );
            }
        }
    }

    bool CriariaCombinacaoInicial(
        int linha,
        int coluna,
        int tipo
    )
    {
        if (
            coluna >= 2 &&
            tipos[linha, coluna - 1] == tipo &&
            tipos[linha, coluna - 2] == tipo
        )
        {
            return true;
        }

        if (
            linha >= 2 &&
            tipos[linha - 1, coluna] == tipo &&
            tipos[linha - 2, coluna] == tipo
        )
        {
            return true;
        }

        return false;
    }

    // =========================================================
    // CRIAR PEÇA
    // =========================================================

    void CriarPeca(
        int linha,
        int coluna,
        int tipo
    )
    {
        GameObject prefab =
            ObterPrefab(tipo);

        if (prefab == null)
            return;

        GameObject novaPeca =
            Instantiate(
                prefab,
                areaTabuleiro
            );

        novaPeca.name =
            "Peca_" +
            linha +
            "_" +
            coluna;

        Peca componente =
            novaPeca.GetComponent<Peca>();

        if (componente != null)
        {
            componente.Configurar(
                this,
                linha,
                coluna
            );
        }

        pecas[linha, coluna] =
            novaPeca;

        tipos[linha, coluna] =
            tipo;

        ColocarNoLugar(
            novaPeca,
            linha,
            coluna
        );
    }

    GameObject ObterPrefab(int tipo)
    {
        if (
            prefabsPecas == null ||
            prefabsPecas.Length == 0
        )
        {
            Debug.LogError(
                "PIMBA CRUSH: Não existem prefabs."
            );

            return null;
        }

        if (
            tipo < 0 ||
            tipo >= prefabsPecas.Length
        )
        {
            Debug.LogError(
                "PIMBA CRUSH: Tipo inválido: " +
                tipo
            );

            return null;
        }

        return prefabsPecas[tipo];
    }

    void ColocarNoLugar(
        GameObject objeto,
        int linha,
        int coluna
    )
    {
        if (
            objeto == null ||
            areaTabuleiro == null
        )
        {
            return;
        }

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        if (rect == null)
            return;

        float largura =
            areaTabuleiro.rect.width /
            colunas;

        float altura =
            areaTabuleiro.rect.height /
            linhas;

        float x =
            -areaTabuleiro.rect.width / 2f +
            largura * coluna +
            largura / 2f;

        float y =
            areaTabuleiro.rect.height / 2f -
            altura * linha -
            altura / 2f;

        rect.anchoredPosition =
            new Vector2(x, y);

        rect.sizeDelta =
            new Vector2(
                largura,
                altura
            );
    }

    // =========================================================
    // CLIQUE
    // =========================================================

    public void ClicarNaPeca(Peca peca)
    {
        if (
            aProcessar ||
            peca == null
        )
        {
            return;
        }

        if (pecaSelecionada == null)
        {
            pecaSelecionada = peca;
            return;
        }

        if (pecaSelecionada == peca)
        {
            pecaSelecionada = null;
            return;
        }

        if (
            !SaoAdjacentes(
                pecaSelecionada,
                peca
            )
        )
        {
            pecaSelecionada = peca;
            return;
        }

        Peca primeira =
            pecaSelecionada;

        Peca segunda =
            peca;

        pecaSelecionada = null;

        TrocarPecas(
            primeira,
            segunda
        );

        List<Peca> combinacoes =
            EncontrarCombinacoesDasPecas(
                primeira,
                segunda
            );

        if (combinacoes.Count > 0)
        {
            StartCoroutine(
                ProcessarCombinacoes(
                    combinacoes
                )
            );
        }
        else
        {
            TrocarPecas(
                primeira,
                segunda
            );
        }
    }

    // =========================================================
    // ARRASTAR
    // =========================================================

    public void ArrastarPeca(
        Peca peca,
        int direcaoLinha,
        int direcaoColuna
    )
    {
        if (
            aProcessar ||
            peca == null
        )
        {
            return;
        }

        int novaLinha =
            peca.linha +
            direcaoLinha;

        int novaColuna =
            peca.coluna +
            direcaoColuna;

        if (
            novaLinha < 0 ||
            novaLinha >= linhas ||
            novaColuna < 0 ||
            novaColuna >= colunas
        )
        {
            return;
        }

        GameObject objetoVizinho =
            pecas[
                novaLinha,
                novaColuna
            ];

        if (objetoVizinho == null)
            return;

        Peca vizinha =
            objetoVizinho.GetComponent<Peca>();

        if (vizinha == null)
            return;

        TrocarPecas(
            peca,
            vizinha
        );

        List<Peca> combinacoes =
            EncontrarCombinacoesDasPecas(
                peca,
                vizinha
            );

        if (combinacoes.Count > 0)
        {
            StartCoroutine(
                ProcessarCombinacoes(
                    combinacoes
                )
            );
        }
        else
        {
            TrocarPecas(
                peca,
                vizinha
            );
        }
    }

    // =========================================================
    // ADJACÊNCIA
    // =========================================================

    bool SaoAdjacentes(
        Peca a,
        Peca b
    )
    {
        int diferencaLinha =
            Mathf.Abs(
                a.linha - b.linha
            );

        int diferencaColuna =
            Mathf.Abs(
                a.coluna - b.coluna
            );

        return
            diferencaLinha +
            diferencaColuna == 1;
    }

    // =========================================================
    // TROCAR
    // =========================================================

    void TrocarPecas(
        Peca a,
        Peca b
    )
    {
        int linhaA = a.linha;
        int colunaA = a.coluna;

        int linhaB = b.linha;
        int colunaB = b.coluna;

        GameObject objetoA =
            pecas[
                linhaA,
                colunaA
            ];

        GameObject objetoB =
            pecas[
                linhaB,
                colunaB
            ];

        int tipoA =
            tipos[
                linhaA,
                colunaA
            ];

        int tipoB =
            tipos[
                linhaB,
                colunaB
            ];

        pecas[
            linhaA,
            colunaA
        ] = objetoB;

        pecas[
            linhaB,
            colunaB
        ] = objetoA;

        tipos[
            linhaA,
            colunaA
        ] = tipoB;

        tipos[
            linhaB,
            colunaB
        ] = tipoA;

        a.linha = linhaB;
        a.coluna = colunaB;

        b.linha = linhaA;
        b.coluna = colunaA;

        ColocarNoLugar(
            objetoA,
            linhaB,
            colunaB
        );

        ColocarNoLugar(
            objetoB,
            linhaA,
            colunaA
        );
    }

    // =========================================================
    // COMBINAÇÕES DO MOVIMENTO
    // =========================================================

    List<Peca> EncontrarCombinacoesDasPecas(
        Peca primeira,
        Peca segunda
    )
    {
        HashSet<Peca> resultado =
            new HashSet<Peca>();

        AdicionarCombinacoesDaPeca(
            primeira,
            resultado
        );

        AdicionarCombinacoesDaPeca(
            segunda,
            resultado
        );

        return new List<Peca>(
            resultado
        );
    }

    void AdicionarCombinacoesDaPeca(
        Peca peca,
        HashSet<Peca> resultado
    )
    {
        if (peca == null)
            return;

        int tipo =
            tipos[
                peca.linha,
                peca.coluna
            ];

        List<Peca> horizontal =
            EncontrarLinha(
                peca.linha,
                peca.coluna,
                tipo
            );

        if (horizontal.Count >= 3)
        {
            foreach (Peca p in horizontal)
            {
                if (p != null)
                    resultado.Add(p);
            }
        }

        List<Peca> vertical =
            EncontrarColuna(
                peca.linha,
                peca.coluna,
                tipo
            );

        if (vertical.Count >= 3)
        {
            foreach (Peca p in vertical)
            {
                if (p != null)
                    resultado.Add(p);
            }
        }
    }

    // =========================================================
    // LINHA
    // =========================================================

    List<Peca> EncontrarLinha(
        int linha,
        int coluna,
        int tipo
    )
    {
        List<Peca> resultado =
            new List<Peca>();

        for (
            int c = coluna;
            c >= 0;
            c--
        )
        {
            if (
                tipos[linha, c] != tipo
            )
            {
                break;
            }

            if (
                pecas[linha, c] != null
            )
            {
                Peca p =
                    pecas[
                        linha,
                        c
                    ].GetComponent<Peca>();

                if (p != null)
                    resultado.Add(p);
            }
        }

        for (
            int c = coluna + 1;
            c < colunas;
            c++
        )
        {
            if (
                tipos[linha, c] != tipo
            )
            {
                break;
            }

            if (
                pecas[linha, c] != null
            )
            {
                Peca p =
                    pecas[
                        linha,
                        c
                    ].GetComponent<Peca>();

                if (p != null)
                    resultado.Add(p);
            }
        }

        return resultado;
    }

    // =========================================================
    // COLUNA
    // =========================================================

    List<Peca> EncontrarColuna(
        int linha,
        int coluna,
        int tipo
    )
    {
        List<Peca> resultado =
            new List<Peca>();

        for (
            int l = linha;
            l >= 0;
            l--
        )
        {
            if (
                tipos[l, coluna] != tipo
            )
            {
                break;
            }

            if (
                pecas[l, coluna] != null
            )
            {
                Peca p =
                    pecas[
                        l,
                        coluna
                    ].GetComponent<Peca>();

                if (p != null)
                    resultado.Add(p);
            }
        }

        for (
            int l = linha + 1;
            l < linhas;
            l++
        )
        {
            if (
                tipos[l, coluna] != tipo
            )
            {
                break;
            }

            if (
                pecas[l, coluna] != null
            )
            {
                Peca p =
                    pecas[
                        l,
                        coluna
                    ].GetComponent<Peca>();

                if (p != null)
                    resultado.Add(p);
            }
        }

        return resultado;
    }

    // =========================================================
    // PROCURAR CASCATAS
    // =========================================================

    List<Peca> EncontrarTodasAsCombinacoes()
    {
        HashSet<Peca> resultado =
            new HashSet<Peca>();

        for (
            int linha = 0;
            linha < linhas;
            linha++
        )
        {
            for (
                int coluna = 0;
                coluna < colunas;
                coluna++
            )
            {
                if (
                    pecas[
                        linha,
                        coluna
                    ] == null
                )
                {
                    continue;
                }

                Peca peca =
                    pecas[
                        linha,
                        coluna
                    ].GetComponent<Peca>();

                if (peca == null)
                    continue;

                int tipo =
                    tipos[
                        linha,
                        coluna
                    ];

                List<Peca> horizontal =
                    EncontrarLinha(
                        linha,
                        coluna,
                        tipo
                    );

                if (horizontal.Count >= 3)
                {
                    foreach (Peca p in horizontal)
                    {
                        if (p != null)
                            resultado.Add(p);
                    }
                }

                List<Peca> vertical =
                    EncontrarColuna(
                        linha,
                        coluna,
                        tipo
                    );

                if (vertical.Count >= 3)
                {
                    foreach (Peca p in vertical)
                    {
                        if (p != null)
                            resultado.Add(p);
                    }
                }
            }
        }

        return new List<Peca>(
            resultado
        );
    }

    // =========================================================
    // PROCESSAR COMBINAÇÕES
    // =========================================================

    IEnumerator ProcessarCombinacoes(
        List<Peca> combinacoesIniciais
    )
    {
        aProcessar = true;

        List<Peca> combinacoes =
            combinacoesIniciais;

        // IMPORTANTE:
        // limite absoluto para nunca bloquear o Unity
        for (
            int cascata = 0;
            cascata < MAX_CASCATAS;
            cascata++
        )
        {
            if (
                combinacoes == null ||
                combinacoes.Count == 0
            )
            {
                break;
            }

            HashSet<Peca> unicas =
                new HashSet<Peca>();

            foreach (Peca p in combinacoes)
            {
                if (p != null)
                    unicas.Add(p);
            }

            if (unicas.Count == 0)
                break;

            // ---------------------------------------------
            // DESAPARECER
            // ---------------------------------------------

            foreach (Peca p in unicas)
            {
                if (p != null)
                {
                    StartCoroutine(
                        p.FadeOut(0.15f)
                    );
                }
            }

            yield return new WaitForSeconds(
                0.17f
            );

            // ---------------------------------------------
            // REMOVER
            // ---------------------------------------------

            foreach (Peca p in unicas)
            {
                if (p == null)
                    continue;

                int linha = p.linha;
                int coluna = p.coluna;

                if (
                    linha >= 0 &&
                    linha < linhas &&
                    coluna >= 0 &&
                    coluna < colunas &&
                    pecas[
                        linha,
                        coluna
                    ] == p.gameObject
                )
                {
                    pecas[
                        linha,
                        coluna
                    ] = null;

                    tipos[
                        linha,
                        coluna
                    ] = -1;
                }

                Destroy(
                    p.gameObject
                );
            }

            // ---------------------------------------------
            // FAZER CAIR
            // ---------------------------------------------

            for (
                int coluna = 0;
                coluna < colunas;
                coluna++
            )
            {
                int destino =
                    linhas - 1;

                for (
                    int linha = linhas - 1;
                    linha >= 0;
                    linha--
                )
                {
                    if (
                        pecas[
                            linha,
                            coluna
                        ] == null
                    )
                    {
                        continue;
                    }

                    if (linha != destino)
                    {
                        GameObject objeto =
                            pecas[
                                linha,
                                coluna
                            ];

                        int tipo =
                            tipos[
                                linha,
                                coluna
                            ];

                        pecas[
                            destino,
                            coluna
                        ] = objeto;

                        tipos[
                            destino,
                            coluna
                        ] = tipo;

                        pecas[
                            linha,
                            coluna
                        ] = null;

                        tipos[
                            linha,
                            coluna
                        ] = -1;

                        Peca peca =
                            objeto.GetComponent<Peca>();

                        if (peca != null)
                        {
                            peca.linha =
                                destino;

                            peca.coluna =
                                coluna;
                        }

                        ColocarNoLugar(
                            objeto,
                            destino,
                            coluna
                        );
                    }

                    destino--;
                }

                // -----------------------------------------
                // NOVAS PEÇAS
                // -----------------------------------------

                for (
                    int linha = destino;
                    linha >= 0;
                    linha--
                )
                {
                    int tipo =
                        Random.Range(
                            0,
                            prefabsPecas.Length
                        );

                    CriarPeca(
                        linha,
                        coluna,
                        tipo
                    );
                }
            }

            yield return new WaitForSeconds(
                0.12f
            );

            // -----------------------------------------
            // VER SE HOUVE CASCATA
            // -----------------------------------------

            combinacoes =
                EncontrarTodasAsCombinacoes();
        }

        // SEGURANÇA FINAL
        pecaSelecionada = null;
        aProcessar = false;

        Debug.Log(
            "PIMBA CRUSH: processamento terminado."
        );
    }
}