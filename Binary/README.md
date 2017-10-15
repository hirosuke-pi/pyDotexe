# [2017/08/25 - v0.10-Beta : First-Update]
 + 初回バージョン

# [2017/09/16 - v0.2.0-Beta : Hook-Update]
 + 引数ログ機能の追加
 + モジュール検索機能の強化 (module-hooks機能追加)
 + UIの一部変更
 + バージョン表記の変更: v0.20-Beta → v0.2.0-Beta
 
# [2017/09/18 - v0.3.0-Beta : Add-on-Update]
 + module-hook機能の強化
 + -hooksコマンドの追加
 + module-hooksのオンラインダウンロードに対応 (--upgrade)
 + 引数ログの最大保存数を20から30に引き上げ
 + --eyeコマンドのバグ修正
 + 一部の表示修正

# [2017/09/19 - v0.3.1-Beta]
 + バグ修正
 + -hooks --clearコマンド追加

# [2017/09/22 - v0.3.2-Beta]
 + -excfileコマンドの動作を軽量化

# [2017/09/25 - v0.3.5-Beta]
 + プログラム全体の最適化
 + module-hooksサーバーの変更
 + module-hooksのPythonバージョン管理機能追加
 + バグ修正・表示修正

# [2017/09/28 - v0.4.0-Beta : Plus-Update]
 + フォルダからファイルのみを選択する-pydirfileコマンドの追加
 + -pydirの仕様変更
 + module-hooksの自動ダウンロード機能追加
 + module-hooksで複数指定を可能に (module_name/module_name...)
 + 複数ファイルを出力するコマンド追加 (--no-onefile)
 + バグ修正・表示修正

# [2017/09/29 - v0.4.1-Beta]
 + -build --check-onlyのコマンド追加 (モジュールパス・モジュール名一覧表示)
 + module-hooksで最後に^がつくとすべてのファイルを検索するコマンドの追加

# [2017/10/01 - v0.4.2-Beta]
 + コード圧縮機能追加 (--optimize)


# [2017/10/00 - v1.0.0-Hydrogen : Standerd-Update]
 + *.dllファイル解析時のメモリ解放を最適化
 + module-hooksのサーバーを変更
 + 最低必須OSバージョンを、WindowsXPまで対応 (.NET Framework 4.0に対応)
 + -pydirdirコマンドの追加
 + -pydir-regex, -pyfile-regexコマンド追加 (正規表現コマンド)
 + -excdir, -excfileの廃止、-exclude(正規表現)に統合
 + マルチタスクによるモジュール検索の高速化
 + setup.pyに対応
 + バージョン手動設定コマンド追加
 + pkg_resourcesに対応
 + Python 1.6.xに対応
 + ショートコマンドに対応
 + ヘルプ画面・ヘルプ機能を改良
 + 一部のコマンド名を変更
 + バグ修正・表示修正

