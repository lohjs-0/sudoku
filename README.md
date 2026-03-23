# 🎮 Sudoku WPF

Jogo de Sudoku desenvolvido em **C# com WPF**, com geração automática de puzzles, múltiplos níveis de dificuldade e interface gráfica nativa para Windows.

---

## 📸 Preview

<img src="c5ee2be0-1cfd-11f1-b826-57be7c88f6db.png" alt="Banner de Mitologia" width="100%">
---

## 🚀 Funcionalidades

- ♾️ **Geração automática de puzzles** — cada partida é única, gerada com backtracking
- 🎯 **3 níveis de dificuldade:**
  - ☕ **Café com Leite** — mais de 36 pistas, ideal para iniciantes
  - 😐 **Normal** — entre 27 e 36 pistas, desafiador
  - 🚨 **Deploy Sexta 18h** — entre 19 e 26 pistas, para os corajosos
- ❌ **Erros marcados em vermelho** em tempo real
- ⏱️ **Cronômetro** que para ao vencer
- ⌨️ **Teclado e mouse** — digite pelo teclado ou clique no numpad
- 🧭 **Navegação pelas setas** do teclado
- 🔦 **Destaque de linha, coluna e bloco** ao selecionar uma célula
- ✅ **Botão Verificar** — confere se há erros ou células vazias
- 💡 **Botão Resolver** — revela a solução completa
- 🧹 **Botão Limpar** — apaga todas as respostas do jogador

---

## 🛠️ Tecnologias utilizadas

| Tecnologia | Uso |
|------------|-----|
| **C#** | Linguagem principal |
| **WPF** | Interface gráfica nativa Windows |
| **XAML** | Definição do layout visual |
| **.NET SDK** | Plataforma de execução |
| **Backtracking** | Geração e validação de puzzles |
| **Fisher-Yates Shuffle** | Embaralhamento para puzzles únicos |
| **async/await** | Geração em background sem travar a UI |
| **DispatcherTimer** | Cronômetro em tempo real |

---

## 📁 Estrutura do projeto

```
SudokuWPF/
├── SudokuEngine.cs       # Lógica do jogo (geração, validação, solução)
├── MainWindow.xaml       # Interface gráfica (layout e estilos)
├── MainWindow.xaml.cs    # Conexão entre interface e lógica
└── SudokuWPF.csproj      # Configuração do projeto .NET
```

---

## ⚙️ Como rodar

### Pré-requisitos
- Windows 10 ou superior
- [.NET SDK 8+](https://dotnet.microsoft.com/download)

### Passos

```bash
# Clone o repositório
git clone https://github.com/SEU_USUARIO/SudokuWPF.git

# Entre na pasta
cd SudokuWPF/SudokuWPF

# Rode o projeto
dotnet run
```

---

## 🧩 Como jogar

1. Escolha a dificuldade no topo da janela
2. Clique em **Nova Partida** para gerar um puzzle
3. Clique em uma célula vazia para selecioná-la
4. Digite o número pelo **teclado** ou clique no **numpad**
5. Números errados aparecem em **vermelho** automaticamente
6. Use as **setas do teclado** para navegar entre as células
7. Clique em **Verificar** para checar sua solução
8. Boa sorte no **Deploy Sexta 18h**! 🚨

---

## 🧠 Como funciona o gerador de puzzles

1. **Preenche** uma grade 9x9 completamente válida usando backtracking recursivo com números embaralhados
2. **Salva** a solução completa
3. **Remove** células aleatoriamente, verificando que o puzzle ainda tem solução única
4. **Marca** as células restantes como fixas (não editáveis pelo jogador)

---

## 📚 O que aprendi

- Algoritmo de **backtracking** para geração e resolução de puzzles
- Criação de **interfaces gráficas** com WPF e XAML
- Uso de **async/await** para não travar a UI durante processamentos pesados
- Organização de código separando **lógica** da **interface**
- Estruturas de dados como **matrizes bidimensionais** e **listas de tuplas**
- Controle de versão com **Git e GitHub**
