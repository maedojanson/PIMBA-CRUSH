using UnityEngine;
using System.Collections.Generic;

public class TabuleiroManager : MonoBehaviour
{
    public int linhas = 6;
    public int colunas = 6;

    public GameObject[] prefabsPecas;
    public Transform paiDoTabuleiro;

    public float distancia = 100f;

    private GameObject[,] pecas;
    private int[,] tipos;

    private bool aProcessar = false;

    void Start()
    {
        if (prefabsPecas == null ||
            prefabsPecas.Length == 0)
        {
            Debug.LogError(
                "PIMBA CRUSH: Não existem prefabs de peças ligados!"
            );
            return;
        }

        CriarTabuleiro();
    }

    void CriarTabuleiro()
    {
        pecas = new GameObject[linhas, colunas];
        tipos = new int[linhas, colunas];

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                int tipo = EscolherTipoInicial(l, c);

                CriarPeca(
                    l,
                    c,
                    tipo
                );
            }
        }
    }

    int EscolherTipoInicial(int l, int c)
    {
        List<int> disponiveis =
            new List<int>();

        for (int i = 0; i < prefabsPecas.Length; i++)
        {
            if (!CriariaTrio(l, c, i))
            {
                disponiveis.Add(i);
            }
        }

        if (disponiveis.Count == 0)
        {
            return Random.Range(
                0,
                prefabsPecas.Length
            );
        }

        return disponiveis[
            Random.Range(
                0,
                disponiveis.Count
            )
        ];
    }

    bool CriariaTrio(
        int l,
        int c,
        int tipo
    )
    {
        if (c >= 2)
        {
            if (tipos[l, c - 1] == tipo &&
                tipos[l, c - 2] == tipo)
            {
                return true;
            }
        }

        if (l >= 2)
        {
            if (tipos[l - 1, c] == tipo &&
                tipos[l - 2, c] == tipo)
            {
                return true;
            }
        }

        return false;
    }

    void CriarPeca(
        int l,
        int c,
        int tipo
    )
    {
        if (l < 0 || l >= linhas)
            return;

        if (c < 0 || c >= colunas)
            return;

        if (tipo < 0 ||
            tipo >= prefabsPecas.Length)
            return;

        if (prefabsPecas[tipo] == null)
            return;

        GameObject novaPeca;

        if (paiDoTabuleiro != null)
        {
            novaPeca = Instantiate(
                prefabsPecas[tipo],
                paiDoTabuleiro
            );
        }
        else
        {
            novaPeca = Instantiate(
                prefabsPecas[tipo]
            );
        }

        novaPeca.transform.localPosition =
            new Vector3(
                c * distancia,
                -l * distancia,
                0
            );

        Peca peca =
            novaPeca.GetComponent<Peca>();

        if (peca == null)
        {
            peca =
                novaPeca.AddComponent<Peca>();
        }

        peca.Configurar(
            this,
            l,
            c
        );

        pecas[l, c] =
            novaPeca;

        tipos[l, c] =
            tipo;
    }

    public void ClicarNaPeca(Peca peca)
    {
        if (aProcessar)
            return;
    }

    public void ArrastarPeca(
        Peca peca,
        int direcaoLinha,
        int direcaoColuna
    )
    {
        if (aProcessar)
            return;

        if (peca == null)
            return;

        int novaLinha =
            peca.linha +
            direcaoLinha;

        int novaColuna =
            peca.coluna +
            direcaoColuna;

        if (novaLinha < 0 ||
            novaLinha >= linhas ||
            novaColuna < 0 ||
            novaColuna >= colunas)
        {
            return;
        }

        if (pecas[novaLinha, novaColuna] == null)
            return;

        Peca outra =
            pecas[novaLinha, novaColuna]
            .GetComponent<Peca>();

        if (outra == null)
            return;

        TrocarPecas(
            peca,
            outra
        );

        List<Peca> combinacoes =
            EncontrarCombinacoes();

        if (combinacoes.Count >= 3)
        {
            aProcessar = true;

            SubstituirCombinacoes(
                combinacoes
            );

            aProcessar = false;
        }
        else
        {
            TrocarPecas(
                peca,
                outra
            );
        }
    }

    void TrocarPecas(
        Peca a,
        Peca b
    )
    {
        int linhaA =
            a.linha;

        int colunaA =
            a.coluna;

        int linhaB =
            b.linha;

        int colunaB =
            b.coluna;

        GameObject objetoA =
            pecas[linhaA, colunaA];

        GameObject objetoB =
            pecas[linhaB, colunaB];

        int tipoA =
            tipos[linhaA, colunaA];

        int tipoB =
            tipos[linhaB, colunaB];

        pecas[linhaA, colunaA] =
            objetoB;

        pecas[linhaB, colunaB] =
            objetoA;

        tipos[linhaA, colunaA] =
            tipoB;

        tipos[linhaB, colunaB] =
            tipoA;

        a.Configurar(
            this,
            linhaB,
            colunaB
        );

        b.Configurar(
            this,
            linhaA,
            colunaA
        );

        Posicionar(a);
        Posicionar(b);
    }

    void Posicionar(Peca peca)
    {
        if (peca == null)
            return;

        peca.transform.localPosition =
            new Vector3(
                peca.coluna * distancia,
                -peca.linha * distancia,
                0
            );
    }

    List<Peca> EncontrarCombinacoes()
    {
        List<Peca> resultado =
            new List<Peca>();

        // HORIZONTAL

        for (int l = 0; l < linhas; l++)
        {
            for (
                int c = 0;
                c < colunas - 2;
                c++
            )
            {
                if (
                    tipos[l, c] ==
                    tipos[l, c + 1] &&
                    tipos[l, c] ==
                    tipos[l, c + 2]
                )
                {
                    Adicionar(
                        resultado,
                        l,
                        c
                    );

                    Adicionar(
                        resultado,
                        l,
                        c + 1
                    );

                    Adicionar(
                        resultado,
                        l,
                        c + 2
                    );
                }
            }
        }

        // VERTICAL

        for (int c = 0; c < colunas; c++)
        {
            for (
                int l = 0;
                l < linhas - 2;
                l++
            )
            {
                if (
                    tipos[l, c] ==
                    tipos[l + 1, c] &&
                    tipos[l, c] ==
                    tipos[l + 2, c]
                )
                {
                    Adicionar(
                        resultado,
                        l,
                        c
                    );

                    Adicionar(
                        resultado,
                        l + 1,
                        c
                    );

                    Adicionar(
                        resultado,
                        l + 2,
                        c
                    );
                }
            }
        }

        return resultado;
    }

    void Adicionar(
        List<Peca> lista,
        int l,
        int c
    )
    {
        if (l < 0 ||
            l >= linhas)
            return;

        if (c < 0 ||
            c >= colunas)
            return;

        if (pecas[l, c] == null)
            return;

        Peca peca =
            pecas[l, c]
            .GetComponent<Peca>();

        if (peca != null &&
            !lista.Contains(peca))
        {
            lista.Add(peca);
        }
    }

    void SubstituirCombinacoes(
        List<Peca> combinacoes
    )
    {
        foreach (Peca peca in combinacoes)
        {
            if (peca == null)
                continue;

            int l =
                peca.linha;

            int c =
                peca.coluna;

            if (l < 0 ||
                l >= linhas ||
                c < 0 ||
                c >= colunas)
                continue;

            GameObject antiga =
                pecas[l, c];

            if (antiga != null)
            {
                Destroy(
                    antiga
                );
            }

            pecas[l, c] =
                null;
        }

        foreach (Peca peca in combinacoes)
        {
            if (peca == null)
                continue;

            int l =
                peca.linha;

            int c =
                peca.coluna;

            int tipo =
                Random.Range(
                    0,
                    prefabsPecas.Length
                );

            CriarPeca(
                l,
                c,
                tipo
            );
        }
    }
}
