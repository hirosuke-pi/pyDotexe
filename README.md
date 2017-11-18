# pyDotexe
pyDotexeは、Pythonで作成したプログラムを実行ファイル形式にできるツールです。

# <５つの特徴>

## 起動を早く
+ 他のツールにはないキャッシュ機能により、単体で、より高速にプログラムを実行することができます。Pythonの実行環境に近い状態で起動しているので、安全に起動することができます。

## 幅広いバージョンサポート
+ 内部コードを極力C#にすることで、Pythonのバージョン違いによるプログラム停止を防ぎ、このツールだけで幅広いPythonのバージョンをサポートすることができました。

## 気になるデータサイズ
+ プログラム全体の圧縮に加え、Pythonのコード圧縮機能によりさらにサイズを抑えることができました。また、ユーザーの指定で圧縮度・圧縮速度の変更を自由に変更できます。

## モジュール・サーチ
+ Module-Hooks機能により、わざわざ引数からモジュールを入力する作業を削減することができました。また、デバッグ機能により、どのモジュールが足りないかすぐわかるようにしました。

## 汎用性を重視
+ 実行ファイルにするだけでなく、集めたモジュールのZIPファイルを出力したり、単体で出力できない他のツールで生成されたファイルらを、それらをまとめて一つのアプリケーションとして実行することもできます。

# <インストールの仕方>

1. pydotexe.exeを、PythonのScriptsフォルダにコピー

2. py2exeのように、pydotexe.exeのsetup.pyを作成したい場合↓
>pip install pydotexe

# <setup.pyの書き方>

+ pip install pydotexe のコマンドでインストールしたモジュールを使って、py2exeのようなsetup.pyを作ることができます。

>import pydotexe.setup

>build_set = pydotexe.setup.build("yourcode.py")

>#build_set.hide_console = True

>#build_set.icon = ""

>build_set.start_build()


# <サポート・バージョン情報>

## Python

+ 動作確認済み: Python 1.6.x, Python 2.1.x ~ Python 2.7.x, Python 3.0.x ~ Python 3.6.x

## Module-Hooks

+ pyDotexe v0.1.0 ~ v1.0.0: Module-Hooks_v1.x.x

+ pyDotexe v2.0.0 ~ : Module-Hooks_v2.x.x

## Windows

+ このプログラムと作成したアプリケーションの動作環境: Windows XP 以降、.NET Framework 4.0 がインストール済み

+ 動作確認済み: WindowsXP, Windows7, Windows10

# <注意事項>

+ pyDotexe本体が、PythonのフォルダのScriptsフォルダ内にある時のみ、-pyコマンドでのPythonのフォルダ指定は必要ありません。それ以外のフォルダにある場合は必ず指定してください。

+ 環境変数にてPythonのフォルダパスを登録した状態で実行してください。

+ Module-Hooksのモジュールデータはテスト中です。

+ Python 2.4.x以下では、一部制限があります。

+ Module-Hooks v1.x.xとv2.x.xとの互換性はありません。

# <プログラムのコマンド説明>

### v2.1.x-Unityのコマンド一覧です。

>pydotexe -build -src yourcode.py

>pydotexe -build -src yourcode.py --hide -icon myicon.ico

>pydotexe -merge -src test.zip -start-bin python.exe -start-src yourcode.py

>pydotexe -hooks -add matplotlib pydir mpl-data 

## -build [Options...]

|コマンド|引数|説明|
|:--:|:--:|:--:|
|-src|ソースコードファイルパス|ソースコードの指定|
|(-py)|Pythonのフォルダパス|Pythonのフォルダパスを指定
|(-file)|ファイルパス|指定されたモジュールファイルパスを追加
|(-dir)|フォルダパス|指定されたモジュールフォルダを追加
|(-pyfile)|正規表現|Python内のモジュールファイルを追加
|(-pydir)|正規表現|Python内のモジュールフォルダを追加
|(-pymodule)|モジュール名|指定されたモジュールを検索し追加
|(-resfile)|ファイル名|リソースファイルにしたいファイルを追加
|(-resdir)|フォルダ名|リソースフォルダにしたいフォルダを追加
|(-exclude)|正規表現|指定されたモジュールファイルはインポートしない
|(-out)|ファイルパス|アプリケーションを出力するファイル名の指定
|(-icon)|ファイルパス|アプリケーションのアイコン指定
|(-argv)|データ値|固定したいパラメータを設定
|(-pyver)|数値|Pythonのバージョンを手動設定 (ex: 3.1.x -> 31, 1.6.x -> 16)
|(-comp=)|圧縮レベル(int)|圧縮度を設定(0~9)
---
|任意コマンド|説明|
|:--:|:--:|
|--zip-out|ZIP形式で出力する
|--no-cache|キャッシュ機能を無効にする
|--dll-out|PythonのDLLファイルを含めない
|--no-onefile|複数のファイルを出力する
|--debug|モジュールインポートテスト用コマンド
|--check-only|モジュールに関しての情報のみ出力
|--no-hooks|Module-Hooks機能を使用しない
|--part-enc|一部のエンコードモジュールのみインポート
|--wait-key|終了時にキー入力を求める
|--no-log|ログを出力しない
|--all-include|Pythonの実行環境すべて含める
|--hide|コンソール表示を無効にする
|--no-rev|ファイルパス修正コードを加えない
|--research|エラーにより無視されたモジュールを強制検索
|--optimize|モジュール全体のコードを圧縮する(テスト版)
|--all-search|すべてのファイルを調べる
|--no-standalone|インストールされているPythonの実行環境に依存する
---
---

## -merge [Options...]

|コマンド|引数|説明|
|:--:|:--:|:--:|
|-src|ファイル・フォルダパス|--no-onefile時はフォルダパス、通常時はZIPファイルパス
|-start-bin|ファイル名|起動時に起動する実行ファイル名
|(-start-src)|ファイル名|起動時に起動するソースコードファイル名
|(-start-argv)|データ値|固定パラメータの設定
|(-ver)|数値|起動時に使用するPythonのバージョン(3.6.x->36, 2.7.x->27)
|(-icon)|ファイルパス|アプリケーションのアイコンの設定
|(-out)|ファイルパス|出力するファイルパス
|
---
|任意コマンド|説明|
|:--:|:--:|
|--no-cache|キャッシュ機能を無効にする
|--no-log|ログを出力しない
|--no-onefile|複数のファイルを出力する
|--out-bin|外部アプリケーションに依存する
|--wait-key|終了時にキー入力を求める
---
---

## -hooks [Options...]
|コマンド|引数|説明|
|:--:|:--:|:--:|
|(-update)|フォルダパス|結合したいModule-Hooksデータフォルダ
|(-add)|[モジュール名] [コマンド] [モジュールデータ]|Module-Hooksにデータを追加する
---
|任意コマンド|説明|
|:--:|:--:|
|--upgrade|Module-Hooksをアップグレードする
|--upgrade-clean|Module-Hooksを再インストールする
|--clear|Module-Hooksを初期化する
|--no-log|ログを出力しない
|--wait-key|終了時にキー入力を求める
---
---

### -help (ヘルプページを表示)
### -version (バージョン情報の表示)
