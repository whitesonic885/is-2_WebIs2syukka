using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;
using System.Globalization;

namespace is2syukka
{
	/// <summary>
	/// [is2syukka]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// ADD 2007.04.20 東都）高木 ＣＳＶエントリ（自動出荷登録）の高速化
	//	Check_autoEntry1	３つの関数→１つに
	//	Get_Stodokesaki2	関数のシンプル化
	//	Get_autoEntryPref2	関数のシンプル化
	//	Get_autoEntryClaim2	関数のシンプル化
	//--------------------------------------------------------------------------
	//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応
	//	Get_JurnalNo 採番管理の関数化
	//	自動出荷登録 仕分ＣＤの設定の高速化
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 東都）高木 オブジェクトの破棄
	//	disposeReader(reader);
	//	reader = null;
	// DEL 2007.05.10 東都）高木 未使用関数のコメント化
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	// ADD 2007.07.06 東都）高木 一覧項目に発店ＣＤを表示 
	// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 
	//--------------------------------------------------------------------------
	// MOD 2008.06.12 kcl)森本 着店コード検索方法の変更
	//  Ins_syukka
	//  Upd_syukka
	// ADD 2008.06.12 kcl)森本 着店コード検索方法の変更に伴い、メソッド追加
	//  Get_tyakuten3
	//  GetSED
	//  Get_autoEntryPref3
	// MOD 2008.07.03 東都）高木 得意先情報の再取得 
	// MOD 2008.06.17 東都）高木 運賃が０の場合[＊]表示 
	// ADD 2008.07.09 東都）高木 未発行分を除外する 
	// MOD 2008.10.16 kcl)森本 着店コード等が Empty にならないようにする
	// ADD 2008.10.29 東都）高木 請求先情報を追加 
	// ADD 2008.10.31 東都）高木 着店コード検索時に着店非表示チェックを追加 
	// MOD 2008.11.19 東都）高木 着店コードが空白でもエラーでなくする 
	// MOD 2008.12.24 kcl)森本 着店コードの検索方法を再変更
	//--------------------------------------------------------------------------
	// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 
	// MOD 2009.04.02 東都）高木 稼働日対応 
	// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 
	// MOD 2009.09.11 東都）高木 出荷照会で出荷済ＦＧ,送信済ＦＧなどを追加 
	// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 
	//--------------------------------------------------------------------------
	// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）
	// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 
	// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 
	//保留 MOD 2010.07.21 東都）高木 リコー様対応 
	// MOD 2010.07.30 東都）高木 自動出荷時の荷送人情報取得の訂正 
	// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 
	// MOD 2010.10.13 東都）高木 [品名記事４]など項目追加 
	// MOD 2010.10.27 東都）高木 削除日時などの追加 
	//保留 MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 
	// MOD 2010.11.10 東都）高木 更新者、更新ＰＧの項目の修正 
	// MOD 2010.11.12 東都）高木 未発行データを削除可能にする 
	//--------------------------------------------------------------------------
	// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない 
	// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 
	// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 
	// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 
	// MOD 2011.04.13 東都）高木 重量入力不可対応 
	// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 
	// MOD 2011.07.14 東都）高木 記事行の追加 START
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
	// MOD 2012.10.01 COA)横山 Oracleサーバ負荷軽減対策
	//                         （ORA-01461により、ST01へのINSERTを元のコードに戻す）
	//--------------------------------------------------------------------------
	// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加
	// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加
	//--------------------------------------------------------------------------
	// ADD 2015.07.12 bevas)松本 バーコード読取専用画面の追加
	// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応
	// MOD 2015.12.15 BEVAS) 松本 輸送禁止エリア機能対応
	//--------------------------------------------------------------------------
	// MOD 2016.02.02 BEVAS）松本 荷送人マスタ取得項目追加（重量、才数）
	// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応
	// MOD 2016.06.23 bevas) 松本 社内伝票対応のバグ修正
	//                            （会員の扱店が親店のとき、集約店が空白になる）
	// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応
	//                            （出荷更新・削除処理時は、処理０４を「0」にする）
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2syukka")]

	public class Service1 : is2common.CommService
	{
		private static string sCRLF = "\\r\\n";
		private static string sSepa = "|";
		private static string sKanma = ",";
		private static string sDbl = "\"";
		private static string sSng = "'";
			
		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/*********************************************************************
		 * 出荷一覧取得
		 * 引数：会員ＣＤ、部門ＣＤ、荷受人ＣＤ、荷送人ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日、状態
		 * 戻値：ステータス、一覧（出荷日、住所１、名前１、）...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_1 
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//			= "SELECT NVL(TO_CHAR(COUNT(*) ,'999,999'),'0'), \n"
//			+       " NVL(TO_CHAR(SUM(個数),'999,999'),'0'), \n"
//			+       " NVL(TO_CHAR(SUM(重量),'999,999,999'),'0') \n"
//			+  " FROM \"ＳＴ０１出荷ジャーナル\" S, ＡＭ０３状態 J \n";
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
//			+       " NVL(COUNT(*),0), \n"
			+       " NVL(COUNT(S.ROWID),0), \n"
			+       " NVL(SUM(S.個数),0), \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//// MOD 2011.04.13 東都）高木 重量入力不可対応 START
////			+       " NVL(SUM(S.重量),0), \n"
////// ADD 2005.05.16 東都）小童谷 才数追加 START
////			+       " NVL(SUM(S.才数),0) \n"
////// ADD 2005.05.16 東都）小童谷 才数追加 END
//			+       " NVL(SUM(DECODE(S.運賃重量,'     ',0,S.運賃重量)),0), \n"
//			+       " NVL(SUM(DECODE(S.運賃才数,'     ',0,S.運賃才数)),0), \n"
//			+       " 1 \n"
//// MOD 2011.04.13 東都）高木 重量入力不可対応 END
			+       " NVL(SUM(S.重量),0) \n"
			+       ", NVL(SUM(S.才数),0) \n"
			+       ", NVL(SUM(DECODE(S.運賃重量,'     ',0,S.運賃重量)),0) \n"
			+       ", NVL(SUM(DECODE(S.運賃才数,'     ',0,S.運賃才数)),0) \n"
			+       ", NVL(MAX(CM01.保留印刷ＦＧ),'0') \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
			+  " FROM \"ＳＴ０１出荷ジャーナル\" S, ＡＭ０３状態 J \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END

		private static string GET_SYUKKA_SELECT_2 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " SUBSTR(S.出荷日,5,2) || '/' || SUBSTR(S.出荷日,7,2), S.住所１, S.名前１, \n"
			+       " TO_CHAR(S.個数), S.重量, S.輸送指示１, \n"
			+       " S.品名記事１, S.送り状番号, DECODE(S.元着区分,1,'元払',2,'着払',S.元着区分), \n"
// MOD 2005.06.08 東都）伊賀 指定日区分追加 START
//			+       " DECODE(S.指定日,0,' ',(SUBSTR(S.指定日,5,2) || '/' || SUBSTR(S.指定日,7,2))), \n"
			+       " DECODE(S.指定日,0,' ',(SUBSTR(S.指定日,5,2) || '/' || SUBSTR(S.指定日,7,2) || DECODE(S.指定日区分,'0','必着','1','指定',''))), \n"
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
//			+       " DECODE(状態,1,'登録済',2,'発行済',3,'出荷済',4,'運行中',5,'配完',6,'不在',7,'返品',' '), \n"
//			+       " DECODE(状態,'01','未発行','02','発行済','03','出荷済','04','集荷', \n"
//			+                   " '05','運行中','06','到着'  ,'07','持出'  ,'08','配完', \n"
//			+                   " '09',DECODE(詳細状態,'01','不在','02','返品',詳細状態),状態), \n"

//			+       " DECODE(状態,'09',NVL(J.状態詳細名, ' '), NVL(J.状態名, ' ')), \n"
			+       " DECODE(S.詳細状態,'  ', NVL(J.状態名, S.状態),NVL(J.状態詳細名, S.詳細状態)), \n"
			+       " SUBSTR(S.登録日,5,2) || '/' || SUBSTR(S.登録日,7,2), \n"
			+       " S.お客様出荷番号, TO_CHAR(S.\"ジャーナルＮＯ\"), S.登録日, \n"
			+       " SUBSTR(S.出荷日,1,4) || '/' || SUBSTR(S.出荷日,5,2) || '/' || SUBSTR(S.出荷日,7,2), \n"
// ADD 2005.05.11 東都）小童谷 登録者追加 START
			+       " S.登録者, \n"
// ADD 2005.05.11 東都）小童谷 登録者追加 END
// ADD 2005.05.16 東都）小童谷 才数追加 START
			+       " S.才数 \n"
// ADD 2005.05.16 東都）小童谷 才数追加 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			+       ", S.運賃才数, S.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" S, ＡＭ０３状態 J \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;

		private static string GET_SYUKKA_SELECT_2_SORT
//			= " ORDER BY 出荷日,住所１,名前１,送り状番号,登録日,\"ジャーナルＮＯ\" ";
			= " ORDER BY 登録日,\"ジャーナルＮＯ\" ";

		private static string GET_SYUKKA_SELECT_2_SORT2
//			= " ORDER BY 出荷日,住所１,名前１,送り状番号,登録日,\"ジャーナルＮＯ\" ";
			= " ORDER BY 出荷日,登録日,\"ジャーナルＮＯ\" ";

		[WebMethod]
		public String[] Get_syukka(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷一覧取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			string s登録件数 = "0";
			string s個数合計 = "0";
// DEL 2005.06.08 東都）高木 未使用の為削除 START
//			string s重量合計 = "0";
// DEL 2005.06.08 東都）高木 未使用の為削除 END
			int    i登録件数 = 0;
// ADD 2005.05.16 東都）小童谷 才数追加 START
			decimal d重量合計 = 0;
			decimal d才数合計 = 0;
// ADD 2005.05.16 東都）小童谷 才数追加 END
// ADD 2005.05.18 東都）小童谷 送り状 START
			string s送り状    = "";
// ADD 2005.05.18 東都）小童谷 送り状 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
			decimal d才数重量合計 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			StringBuilder sbRet = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE S.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND S.部門ＣＤ = '" + sBCode + "' \n");

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND S.荷受人ＣＤ = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND S.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND S.荷受人ＣＤ = '"+ sTCode + "' \n");
					sbQuery.Append(" AND S.荷送人ＣＤ = '"+ sICode + "' \n");
				}
// ADD 2005.06.01 東都）小童谷 チョイスは日付範囲なし START
//				if(iSyuka == 0)
//					sbQuery.Append(" AND S.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
//				else
//					sbQuery.Append(" AND S.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				if(sSday != "0")
				{
					if(iSyuka == 0)
						sbQuery.Append(" AND S.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
					else
						sbQuery.Append(" AND S.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				}
// ADD 2005.06.01 東都）小童谷 チョイスは日付範囲なし END
				
				if(sJyoutai != "00")
				{
					if(sJyoutai == "aa")
						sbQuery.Append(" AND S.状態 <> '01' \n");
					else
						sbQuery.Append(" AND S.状態 = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND S.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND S.状態     = J.状態ＣＤ(+) \n");
				sbQuery.Append(" AND S.詳細状態 = J.状態詳細ＣＤ(+) \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(" AND S.会員ＣＤ     = CM01.会員ＣＤ(+) \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				sbQuery2.Append(GET_SYUKKA_SELECT_1);
				sbQuery2.Append(sbQuery);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery2);

				if(reader.Read())
				{
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//					s登録件数   = reader.GetString(0);
//					s個数合計   = reader.GetString(1);
//					s重量合計   = reader.GetString(2);
					s登録件数   = reader.GetDecimal(0).ToString("#,##0").Trim();
					s個数合計   = reader.GetDecimal(1).ToString("#,##0").Trim();
//					s重量合計   = reader.GetDecimal(2).ToString("#,##0").Trim();
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END
// ADD 2005.05.16 東都）小童谷 才数追加 START
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					if(reader.GetString(6) == "1"){
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						d重量合計   = reader.GetDecimal(2);
						d才数合計   = reader.GetDecimal(3);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}else{
						d重量合計   = reader.GetDecimal(4);
						d才数合計   = reader.GetDecimal(5);
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// ADD 2005.05.16 東都）小童谷 才数追加 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				sRet[1] = s登録件数;
				sRet[2] = s個数合計;
// MOD 2005.05.16 東都）小童谷 才数追加 START
//				sRet[3] = s重量合計;
				d重量合計 = d重量合計 + d才数合計 * 8;
				sRet[3] = d重量合計.ToString("#,##0").Trim();
////				if(s重量合計 == "0")
////				{
////					d才数合計 = d才数合計 * 8;
////					sRet[3] = d才数合計.ToString("#,##0").Trim();
////				}
////				else
////				{
////					sRet[3] = s重量合計;
////				}
// MOD 2005.05.16 東都）小童谷 才数追加 END

// MOD 2006.03.09 東都）高木 登録件数が１０００件を超えるとエラーが発生する START
//				i登録件数 = int.Parse(s登録件数);
				i登録件数 = int.Parse(s登録件数.Replace(",",""));
// MOD 2006.03.09 東都）高木 登録件数が１０００件を超えるとエラーが発生する END

				if(i登録件数 == 0)
				{
					sRet[0] = "該当データがありません";
				}
				else
				{
					sRet = new string[i登録件数 + 4];
					sRet[0] = "正常終了";
					sRet[1] = s登録件数;
					sRet[2] = s個数合計;
// MOD 2005.05.16 東都）小童谷 才数追加 START
//					sRet[3] = s重量合計;
					sRet[3] = d重量合計.ToString("#,##0").Trim();
////					if(s重量合計 == "0")
////						sRet[3] = d才数合計.ToString("#,##0").Trim();
////					else
////						sRet[3] = s重量合計;
// MOD 2005.05.16 東都）小童谷 才数追加 END

					sbQuery2 = new StringBuilder(1024);
					if(iSyuka == 0)
					{
						sbQuery2.Append(GET_SYUKKA_SELECT_2);
						sbQuery2.Append(sbQuery);
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
						reader = CmdSelect(sUser, conn2, sbQuery2);
					}
					else
					{
						sbQuery2.Append(GET_SYUKKA_SELECT_2);
						sbQuery2.Append(sbQuery);
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
						reader = CmdSelect(sUser, conn2, sbQuery2);
					}

					int iCnt = 4;
					while (reader.Read() && iCnt < sRet.Length)
					{
						sbRet = new StringBuilder(1024);

						sbRet.Append(sSepa + reader.GetString(0));			// 出荷日
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//						sbRet.Append(sSepa + reader.GetString(1).Trim());	// 住所１
//						sbRet.Append(sCRLF + reader.GetString(2).Trim());	// 名前１
						sbRet.Append(sSepa + reader.GetString(1).TrimEnd()); // 住所１
						sbRet.Append(sCRLF + reader.GetString(2).TrimEnd()); // 名前１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
						sbRet.Append(sSepa + reader.GetString(3));			// 個数
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//// MOD 2005.05.16 東都）小童谷 才数追加 START
////						sbRet.Append(sSepa + reader.GetString(4));			// 重量
//						d才数合計 = reader.GetDecimal(17);
//						d才数合計 = d才数合計 * 8;
//						if(d才数合計 == 0)
//							sbRet.Append(sSepa + reader.GetDecimal(4).ToString("#,##0").Trim()); // 重量
//						else
//							sbRet.Append(sSepa + d才数合計.ToString("#,##0").Trim());		// 才数
//// MOD 2005.05.16 東都）小童谷 才数追加 END
						s運賃才数 = reader.GetString(18).TrimEnd();
						s運賃重量 = reader.GetString(19).TrimEnd();
						d才数重量 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
						s重量入力制御 = reader.GetString(20).TrimEnd();
						if(s重量入力制御 == "1" 
						&& s運賃才数.Length == 0 && s運賃重量.Length == 0
						){
							d才数重量 = reader.GetDecimal(17) * 8;	// 才数
							d才数重量 += reader.GetDecimal(4);		// 重量
							sbRet.Append(sSepa + d才数重量.ToString("#,##0").Trim());
						}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
							if(s運賃才数.Length > 0){
								try{
									d才数重量 += (Decimal.Parse(s運賃才数) * 8);
								}catch(Exception){}
							}
							if(s運賃重量.Length > 0){
								try{
									d才数重量 += Decimal.Parse(s運賃重量);
								}catch(Exception){}
							}
							if(d才数重量 == 0){
								sbRet.Append(sSepa + " ");
							}else{
								sbRet.Append(sSepa + d才数重量.ToString("#,##0").Trim());
							}
//							// お客様入力値
//							d才数合計 = reader.GetDecimal(17) * 8;
//							d才数合計 += reader.GetDecimal(4);
//							if(d才数合計 == 0){
//								;
//							}else{
//								sbRet.Append(sCRLF + "("
//											+ d才数重量.ToString("#,##0").Trim()
//											+ ")");
//							}
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
						}
						d才数重量合計 += d才数重量;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());// 輸送指示１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());	// 品名記事１
						sbRet.Append(sCRLF + reader.GetString(6).TrimEnd()); // 品名記事１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
// ADD 2005.05.18 東都）小童谷 送り状 START
//						sbRet.Append(sSepa + reader.GetString(7).Trim());	// 送り状番号
						s送り状 = reader.GetString(7).Trim();               // 送り状番号
						if(s送り状.Length == 0)
							sbRet.Append(sSepa + s送り状);
						else
							sbRet.Append(sSepa + s送り状.Remove(0,4));
// ADD 2005.05.18 東都）小童谷 送り状 END
						sbRet.Append(sCRLF + reader.GetString(8));			// 元着区分
						sbRet.Append(sSepa + reader.GetString(9));			// 指定日
// MOD 2005.05.11 東都）高木 状態表示のバグ修正 START
//						sbRet.Append(sSepa + reader.GetString(10));			// 状態
						sbRet.Append(sSepa + reader.GetString(10).Trim());	// 状態
// MOD 2005.05.11 東都）高木 状態表示のバグ修正 END
						sbRet.Append(sSepa + reader.GetString(11));			// 登録日
						sbRet.Append(sSepa + reader.GetString(12).Trim());	// お客様出荷番号
						sbRet.Append(sSepa + reader.GetString(13));			// ジャーナルＮＯ
						sbRet.Append(sSepa + reader.GetString(14));			// 登録日
						sbRet.Append(sSepa + reader.GetString(15));			// 出荷日
// ADD 2005.05.11 東都）小童谷 登録者追加 START
						sbRet.Append(sSepa + reader.GetString(16));			// 登録者
// ADD 2005.05.11 東都）小童谷 登録者追加 END
						sbRet.Append(sSepa);
						sRet[iCnt] = sbRet.ToString();
						iCnt++;
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					sRet[3] = d才数重量合計.ToString("#,##0").Trim();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				sbQuery  = null;
				sbQuery2 = null;
				sbRet    = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 出荷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ
		 * 戻値：ステータス、出荷日、お客様出荷番号、荷受人ＣＤ、電話番号...
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_Ssyukka(string[] sUser, string sKCode,string sBCode,string sDay, int iJNo)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷情報取得開始");

// MOD 2005.06.08 東都）伊賀 指定日区分追加 START
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 START
// MOD 2005.05.13 東都）小童谷 荷送人重量追加 START
//			string[] sRet = new string[39];
// MOD 2005.05.17 東都）小童谷 才数追加 START
//			string[] sRet = new string[40];
//			string[] sRet = new string[42];
			OracleConnection conn2 = null;
// MOD 2009.04.02 東都）高木 稼働日対応 START
//			string[] sRet = new string[46];
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 START
//			string[] sRet = new string[47];
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする START
//			string[] sRet = new string[49];
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 START
//			string[] sRet = new string[51];
// MOD 2011.07.14 東都）高木 記事行の追加 START
//			string[] sRet = new string[53];
			string[] sRet = new string[56];
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 END
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする END
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 END
// MOD 2009.04.02 東都）高木 稼働日対応 END
// MOD 2005.05.17 東都）小童谷 才数追加 END
// MOD 2005.05.13 東都）小童谷 荷送人重量追加 END
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 END
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			try
			{
				string cmdQuery = "SELECT /*+ INDEX(S ST01PKEY) INDEX(N SM01PKEY) */ \n"
					+ " substr(J.出荷日,1,4) || '/' || substr(J.出荷日,5,2) || '/' || substr(J.出荷日,7,2), \n"
					+ "J.お客様出荷番号,J.荷受人ＣＤ,J.電話番号１,J.電話番号２,J.電話番号３, \n"
					+ "J.住所１,J.住所２,J.住所３,J.名前１,J.名前２,SUBSTR(J.郵便番号,1,3),SUBSTR(J.郵便番号,4,4), \n"
					+ "J.荷送人ＣＤ,J.部課名,TO_CHAR(J.個数),TO_CHAR(NVL(J.重量,'0')), \n"
					+ "DECODE(J.指定日,0,'0',substr(J.指定日,1,4) || '/' || substr(J.指定日,5,2) || '/' || substr(J.指定日,7,2)), \n"
					+ "J.輸送指示１,J.輸送指示２,J.品名記事１,J.品名記事２,J.品名記事３, \n"
					+ "TO_CHAR(J.保険金額),TO_CHAR(J.更新日時), \n"
					+ "NVL(N.電話番号１,' '),NVL(N.電話番号２,' '),NVL(N.電話番号３,' '), \n"
					+ "NVL(N.住所１,' '),NVL(N.住所２,' '),NVL(N.名前１,' '),NVL(N.名前２,' '), \n"
					+ "NVL(SUBSTR(N.郵便番号,1,3),' '),NVL(SUBSTR(N.郵便番号,4,4),' '), \n"
// MOD 2005.05.13 東都）小童谷 荷送人重量追加 START
//					+ "NVL(N.得意先ＣＤ,' '),NVL(N.得意先部課ＣＤ,' '),J.荷送人部署名 \n"
					+ "NVL(N.得意先ＣＤ,' '),NVL(N.得意先部課ＣＤ,' '),J.荷送人部署名,TO_CHAR(NVL(N.重量,'0')), \n"
// MOD 2005.05.13 東都）小童谷 荷送人重量追加 END
// ADD 2005.05.17 東都）小童谷 才数追加 START
					+ "TO_CHAR(NVL(J.才数,'0')),TO_CHAR(NVL(N.才数,'0')) \n"
// ADD 2005.05.17 東都）小童谷 才数追加 END
// ADD 2005.06.01 東都）伊賀 輸送指示コード追加 START
					+ ",J.輸送指示ＣＤ１,J.輸送指示ＣＤ２,J.送り状番号 \n"
// ADD 2005.06.01 東都）伊賀 輸送指示コード追加 END
// ADD 2005.06.08 東都）伊賀 指定日区分追加 START
					+ ",J.指定日区分 \n"
// ADD 2005.06.08 東都）伊賀 指定日区分追加 END
// MOD 2009.04.02 東都）高木 稼働日対応 START
					+ ",J.登録ＰＧ \n"
// MOD 2009.04.02 東都）高木 稼働日対応 END
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 START
					+ ",J.得意先ＣＤ, J.部課ＣＤ \n"
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 END
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする START
					+ ", J.状態, J.出荷済ＦＧ \n"
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする END
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 START
					+ ", J.送り状発行済ＦＧ, J.送信済ＦＧ \n"
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					+ ", J.品名記事４, J.品名記事５, J.品名記事６ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
					+ " FROM \"ＳＴ０１出荷ジャーナル\" J,ＳＭ０１荷送人 N \n"
					+ " WHERE J.会員ＣＤ   = '" + sKCode + "' \n"
					+ "   AND J.部門ＣＤ   = '" + sBCode + "' \n"
					+ "   AND J.登録日     = '" + sDay + "' \n"
					+ "   AND J.\"ジャーナルＮＯ\" = " + iJNo + " \n"
					+ "   AND J.削除ＦＧ = '0' \n"
					+ "   AND J.荷送人ＣＤ     = N.荷送人ＣＤ(+) \n"
					+ "   AND '" + sKCode + "' = N.会員ＣＤ(+) \n"
					+ "   AND '" + sBCode + "' = N.部門ＣＤ(+) \n"
					+ "   AND '0' = N.削除ＦＧ(+) ";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				bool bRead = reader.Read();
				if(bRead == true)
				{
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 START
// MOD 2005.05.17 東都）小童谷 才数追加 START
//					for(int iCnt = 1; iCnt < 39; iCnt++)
//					for(int iCnt = 1; iCnt < 41; iCnt++)
//					for(int iCnt = 1; iCnt < 44; iCnt++)
					for(int iCnt = 1; iCnt < 45; iCnt++)
// MOD 2005.05.17 東都）小童谷 才数追加 END
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 END
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
					{
						sRet[iCnt] = reader.GetString(iCnt - 1).TrimEnd();
					}
					sRet[0] = "正常終了";
// MOD 2005.06.08 東都）伊賀 指定日区分追加 START
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 START
//					sRet[41] = "U";
//					sRet[44] = "U";
					sRet[45] = "U";
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 END
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
// MOD 2009.04.02 東都）高木 稼働日対応 START
					sRet[46] = reader.GetString(44).TrimEnd();
// MOD 2009.04.02 東都）高木 稼働日対応 END
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 START
					sRet[47] = reader.GetString(45).TrimEnd(); //J.得意先ＣＤ
					sRet[48] = reader.GetString(46).TrimEnd(); //J.部課ＣＤ
// MOD 2010.08.31 東都）高木 現行の請求先情報の表示 END
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする START
					sRet[49] = reader.GetString(47); // 状態
					sRet[50] = reader.GetString(48); // 出荷済ＦＧ
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする END
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 START
					sRet[51] = reader.GetString(49); // 送り状発行済ＦＧ
					sRet[52] = reader.GetString(50); // 送信済ＦＧ
// MOD 2011.03.11 東都）高木 ＧＰ送信済（出荷済）データの修正制限の強化 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					sRet[53] = reader.GetString(51).TrimEnd(); // 品名記事４
					sRet[54] = reader.GetString(52).TrimEnd(); // 品名記事５
					sRet[55] = reader.GetString(53).TrimEnd(); // 品名記事６
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "出荷情報取得["+sBCode+"]["+sDay+"]["+iJNo
									+"]:["+reader.GetString(42).TrimEnd()+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END
				}
				else
				{
					sRet[0] = "該当データがありません";
// MOD 2005.06.08 東都）伊賀 指定日区分追加 START
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 START
//					sRet[41] = "I";
//					sRet[43] = "I";
					sRet[45] = "I";
// MOD 2005.06.01 東都）伊賀 輸送指示コード追加 END
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 出荷データ登録
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_syukka(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷登録開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			decimal d件数;
			string s特殊計 = " ";
			string s登録日;
			int i管理ＮＯ;
			string s日付;

			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			int					iUpdRow			= 0;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			try
			{
				//出荷日チェック
				string[] sSyukkabi = Get_bumonsyukka(sUser, conn2, sData[0], sData[1]);
				sRet[0] = sSyukkabi[0];
				if(sRet[0] != " ") return sRet;
				if(int.Parse(sData[2]) < int.Parse(sSyukkabi[1]))
				{
					sRet[0] = "1";
					sRet[1] = sSyukkabi[1];
					return sRet;
				}

				//荷送人ＣＤ存在チェック
				string cmdQuery
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
////					= "SELECT NVL(COUNT(*),0) \n"
////					= "SELECT COUNT(*) \n"
//// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
////					= "SELECT COUNT(ROWID) \n"
//					= "SELECT 得意先ＣＤ, 得意先部課ＣＤ \n"
//// MOD 2008.07.03 東都）高木 得意先情報の再取得 END
//					+ "  FROM ＳＭ０１荷送人 \n"
//					+ " WHERE 会員ＣＤ   = '" + sData[0]  +"' \n"
//					+ "   AND 部門ＣＤ   = '" + sData[1]  +"' \n"
//					+ "   AND 荷送人ＣＤ = '" + sData[15] +"' \n"
//					+ "   AND 削除ＦＧ   = '0'";
					= "SELECT SM01.得意先ＣＤ, SM01.得意先部課ＣＤ \n"
					+ "     , NVL(CM01.保留印刷ＦＧ,'0') \n"
					+ "  FROM ＳＭ０１荷送人 SM01 \n"
					+ "     , ＣＭ０１会員 CM01 \n"
					+ " WHERE SM01.会員ＣＤ   = '" + sData[0]  +"' \n"
					+ "   AND SM01.部門ＣＤ   = '" + sData[1]  +"' \n"
					+ "   AND SM01.荷送人ＣＤ = '" + sData[15] +"' \n"
					+ "   AND SM01.削除ＦＧ   = '0' \n"
					+ "   AND SM01.会員ＣＤ   = CM01.会員ＣＤ(+) \n"
					;
				string s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END


				// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
				//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

				cmdQuery
					= "SELECT SM01.得意先ＣＤ, SM01.得意先部課ＣＤ \n"
					+ "     , NVL(CM01.保留印刷ＦＧ,'0') \n"
					+ "  FROM ＳＭ０１荷送人 SM01 \n"
					+ "     , ＣＭ０１会員 CM01 \n"
					+ " WHERE SM01.会員ＣＤ   = :p_KaiinCD \n"
					+ "   AND SM01.部門ＣＤ   = :p_BumonCD \n"
					+ "   AND SM01.荷送人ＣＤ = :p_NisouCD \n"
					+ "   AND SM01.削除ＦＧ   = '0' \n"
					+ "   AND SM01.会員ＣＤ   = CM01.会員ＣＤ(+) \n"
					;
				wk_opOraParam = new OracleParameter[3];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0],  ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1],  ParameterDirection.Input);
				wk_opOraParam[2] = new OracleParameter("p_NisouCD", OracleDbType.Char, sData[15], ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
//				reader.Read();
//				d件数   = reader.GetDecimal(0);
				if(reader.Read()){
					d件数 = 1;
					sData[16] = reader.GetString(0);
					sData[17] = reader.GetString(1);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(2);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				}else{
					d件数 = 0;
				}
// MOD 2008.07.03 東都）高木 得意先情報の再取得 END
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if(d件数 == 0)
				{
					sRet[0] = "0";
				}
				else
				{
// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
					cmdQuery
						= "SELECT SM04.得意先部課名 \n"
						+ " FROM ＣＭ０２部門 CM02 \n"
						+    " , ＳＭ０４請求先 SM04 \n"
						+ " WHERE CM02.会員ＣＤ = '" + sData[0] + "' \n"
						+  " AND CM02.部門ＣＤ = '" + sData[1] + "' \n"
						+  " AND CM02.削除ＦＧ = '0' \n"
//						+  " AND SM04.会員ＣＤ = CM02.会員ＣＤ \n"
						+  " AND SM04.郵便番号 = CM02.郵便番号 \n"
						+  " AND SM04.得意先ＣＤ = '" + sData[16] + "' \n"
						+  " AND SM04.得意先部課ＣＤ = '" + sData[17] + "' \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
						+  " AND SM04.会員ＣＤ = CM02.会員ＣＤ \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
						+  " AND SM04.削除ＦＧ = '0' \n"
						;
					// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
					//reader = CmdSelect(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

					cmdQuery
						= "SELECT SM04.得意先部課名 \n"
						+ " FROM ＣＭ０２部門 CM02 \n"
						+    " , ＳＭ０４請求先 SM04 \n"
						+ " WHERE CM02.会員ＣＤ       = :p_KaiinCD \n"
						+  "  AND CM02.部門ＣＤ       = :p_BumonCD \n"
						+  "  AND CM02.削除ＦＧ       = '0' \n"
						+  "  AND SM04.郵便番号       = CM02.郵便番号 \n"
						+  "  AND SM04.得意先ＣＤ     = :p_TokuiCD \n"
						+  "  AND SM04.得意先部課ＣＤ = :p_TokuiBukaCD \n"
						+  "  AND SM04.会員ＣＤ       = CM02.会員ＣＤ \n"
						+  "  AND SM04.削除ＦＧ       = '0' \n"
						;
					wk_opOraParam = new OracleParameter[4];
					wk_opOraParam[0] = new OracleParameter("p_KaiinCD",     OracleDbType.Char, sData[0],  ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_BumonCD",     OracleDbType.Char, sData[1],  ParameterDirection.Input);
					wk_opOraParam[2] = new OracleParameter("p_TokuiCD",     OracleDbType.Char, sData[16], ParameterDirection.Input);
					wk_opOraParam[3] = new OracleParameter("p_TokuiBukaCD", OracleDbType.Char, sData[17], ParameterDirection.Input);

					reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

					if(reader.Read())
					{
						sData[18] = reader.GetString(0);
					}else{
						sData[18] = " ";
					}
					disposeReader(reader);
					reader = null;
// MOD 2008.07.03 東都）高木 得意先情報の再取得 END

					//特殊計取得
					if(sData[4] != " ")
					{
						cmdQuery
							= "SELECT NVL(特殊計,' ') \n"
							+ "  FROM ＳＭ０２荷受人 \n"
							+ " WHERE 会員ＣＤ   = '" + sData[0] +"' \n"
							+ "   AND 部門ＣＤ   = '" + sData[1] +"' \n"
							+ "   AND 荷受人ＣＤ = '" + sData[4] +"' \n"
							+ "   AND 削除ＦＧ   = '0'";

						// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
						//reader = CmdSelect(sUser, conn2, cmdQuery);
						logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

						cmdQuery
							= "SELECT NVL(特殊計,' ') \n"
							+ "  FROM ＳＭ０２荷受人 \n"
							+ " WHERE 会員ＣＤ   = :p_KaiinCD \n"
							+ "   AND 部門ＣＤ   = :p_BumonCD \n"
							+ "   AND 荷受人ＣＤ = :p_NiukeCD \n"
							+ "   AND 削除ＦＧ   = '0'";

						wk_opOraParam = new OracleParameter[3];
						wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
						wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);
						wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sData[4], ParameterDirection.Input);

						reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
						wk_opOraParam = null;
						// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

						bool bRead = reader.Read();
						if(bRead == true)
							s特殊計   = reader.GetString(0);

// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 START
						cmdQuery
							= "UPDATE ＳＭ０２荷受人 \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 START
//							+ " SET 名前３ = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET 登録ＰＧ = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 END
							+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
							+ " AND 部門ＣＤ   = '" + sData[1] +"' \n"
							+ " AND 荷受人ＣＤ = '" + sData[4] +"' \n"
							+ " AND 削除ＦＧ   = '0'";
						try{
							// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
							//int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
							logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

							cmdQuery
								= "UPDATE ＳＭ０２荷受人 \n"
								+ " SET 登録ＰＧ = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
								+ " WHERE 会員ＣＤ = :p_KaiinCD \n"
								+ " AND 部門ＣＤ   = :p_BumonCD \n"
								+ " AND 荷受人ＣＤ = :p_NiukeCD \n"
								+ " AND 削除ＦＧ   = '0'";

							wk_opOraParam	= new OracleParameter[3];
							wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
							wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);
							wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sData[4], ParameterDirection.Input);

							iUpdRow = CmdUpdate(sUser, conn2, cmdQuery, wk_opOraParam);
							wk_opOraParam = null;
							// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
						}catch(Exception){
							;
						}
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 END
					}

//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 START
					//ジャーナルＮＯ取得
					cmdQuery
						= "SELECT \"ジャーナルＮＯ登録日\",\"ジャーナルＮＯ管理\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM ＣＭ０２部門 \n"
						+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
						+ "   AND 部門ＣＤ = '" + sData[1] +"' \n"
// MOD 2007.04.28 東都）高木 オブジェクトの破棄 START
//						+ "   AND 削除ＦＧ = '0'";
						+ "   AND 削除ＦＧ = '0'"
						+ "   FOR UPDATE "
						;
// MOD 2007.04.28 東都）高木 オブジェクトの破棄 END

					// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
					//reader = CmdSelect(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

					cmdQuery
						= "SELECT \"ジャーナルＮＯ登録日\",\"ジャーナルＮＯ管理\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM ＣＭ０２部門 \n"
						+ " WHERE 会員ＣＤ = :p_KaiinCD \n"
						+ "   AND 部門ＣＤ = :p_BumonCD \n"
						+ "   AND 削除ＦＧ = '0'"
						+ "   FOR UPDATE "
						;
					wk_opOraParam = new OracleParameter[2];
					wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);

					reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

					reader.Read();
					s登録日   = reader.GetString(0).Trim();
					i管理ＮＯ = reader.GetInt32(1);
					s日付     = reader.GetString(2);

//					s日付 = DateTime.Today.ToString().Replace("/","").Substring(0,8);
					if(s登録日 == s日付)
						i管理ＮＯ++;
					else
					{
						s登録日 = s日付;
						i管理ＮＯ = 1;
					}

					cmdQuery 
						= "UPDATE ＣＭ０２部門 \n"
						+    "SET \"ジャーナルＮＯ登録日\"  = '" + s登録日 +"', \n"
						+        "\"ジャーナルＮＯ管理\"    = " + i管理ＮＯ +", \n"
						+        "更新ＰＧ                  = '" + sData[32] +"', \n"
						+        "更新者                    = '" + sData[33] +"', \n"
						+        "更新日時                  =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE 会員ＣＤ       = '" + sData[0] +"' \n"
						+ "   AND 部門ＣＤ       = '" + sData[1] +"' \n"
						+ "   AND 削除ＦＧ = '0'";

					// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
					//int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

					cmdQuery 
						= "UPDATE ＣＭ０２部門 \n"
						+    "SET \"ジャーナルＮＯ登録日\"  = :p_TourokuBi, \n"
						+        "\"ジャーナルＮＯ管理\"    = :p_KanriNo, \n"
						+        "更新ＰＧ                 = :p_KoushinPG, \n"
						+        "更新者                   = :p_KoushinSha, \n"
						+        "更新日時                 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE 会員ＣＤ       = :p_KaiinCD \n"
						+ "   AND 部門ＣＤ       = :p_BumonCD \n"
						+ "   AND 削除ＦＧ = '0'";

					wk_opOraParam	= new OracleParameter[6];
					wk_opOraParam[0] = new OracleParameter("p_TourokuBi",  OracleDbType.Char,    s登録日,   ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_KanriNo",    OracleDbType.Decimal, i管理ＮＯ, ParameterDirection.Input);
					wk_opOraParam[2] = new OracleParameter("p_KoushinPG",  OracleDbType.Char,    sData[32], ParameterDirection.Input);
					wk_opOraParam[3] = new OracleParameter("p_KoushinSha", OracleDbType.Char,    sData[33], ParameterDirection.Input);
					wk_opOraParam[4] = new OracleParameter("p_KaiinCD",    OracleDbType.Char,    sData[0],  ParameterDirection.Input);
					wk_opOraParam[5] = new OracleParameter("p_BumonCD",    OracleDbType.Char,    sData[1],  ParameterDirection.Input);

					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
//					string[] sRet2 = Get_JurnalNo(sUser, sData[0], sData[1], sData[32]);
//					if(sRet2[0].Length == 4){
//						s登録日   = sRet2[1];
//						s日付     = sRet2[1];
//						i管理ＮＯ = int.Parse(sRet2[2]);
//					}else{
//						tran.Rollback();
//						return sRet2;
//					}
//					int iUpdRow = 1;
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 END

					//着店取得
// MOD 2008.12.25 kcl)森本 着店コード検索方法の再変更 START
//// MOD 2008.06.12 kcl)森本 着店コード検索方法の変更 START
////					string[] sTyakuten = Get_tyakuten(sUser, conn2, sData[13] + sData[14]);
//					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
//						sData[0], sData[1], sData[4], 
//						sData[13] + sData[14], sData[8] + sData[9] + sData[10]);
//// MOD 2008.06.12 kcl)森本 着店コード検索方法の変更 END
					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
						sData[0], sData[1], sData[4], 
						sData[13] + sData[14], sData[8] + sData[9] + sData[10], sData[11] + sData[12]);
// MOD 2008.12.25 kcl)森本 着店コード検索方法の再変更 END
					sRet[0] = sTyakuten[0];
					if(sRet[0] != " ") return sRet;
// MOD 2008.10.16 kcl)森本 着店コード等が Empty にならないようにする START
//					string s着店ＣＤ = sTyakuten[1];
//					string s着店名   = sTyakuten[2];
//					string s住所ＣＤ = sTyakuten[3];
					string s着店ＣＤ = (sTyakuten[1].Length > 0) ? sTyakuten[1] : " ";
					string s着店名   = (sTyakuten[2].Length > 0) ? sTyakuten[2] : " ";
					string s住所ＣＤ = (sTyakuten[3].Length > 0) ? sTyakuten[3] : " ";
// MOD 2008.10.16 kcl)森本 着店コード等が Empty にならないようにする END
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 START
					if(sData[8].Equals("※※支店止め※※"))
					{
						// 着店コード変換
						string s変換後着店ＣＤ = 半角数字変換(sData[10]);
						sRet[0] = s変換後着店ＣＤ;
						if(sRet[0].Length != 3)
						{
							// 半角変換失敗
							return sRet;
						}

						// 着店決定
						string[] sTyakuten_GeneralDelivery = Get_tyakuten_GeneralDelivery(sUser, conn2, s変換後着店ＣＤ, sData[13] + sData[14]);
						sRet[0] = sTyakuten_GeneralDelivery[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s着店ＣＤ = s変換後着店ＣＤ;
						s着店名   = (sTyakuten_GeneralDelivery[1].Length > 0) ? sTyakuten_GeneralDelivery[1] : " ";
					}
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 END

					//発店取得
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 START
//					string[] sHatuten = Get_hatuten(sData[15]);
//保留 MOD 2010.07.21 東都）高木 リコー様対応 START
					string[] sHatuten = Get_hatuten(sUser, conn2, sData[0], sData[1]);
//					string[] sHatuten = Get_hatuten3(sUser, conn2, sData[0], sData[1], sData[15]);
//保留 MOD 2010.07.21 東都）高木 リコー様対応 END
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 END
					sRet[0] = sHatuten[0];
					if(sRet[0] != " ") return sRet;
					string s発店ＣＤ = sHatuten[1];
					string s発店名   = sHatuten[2];

					//集荷店取得
					string[] sSyuyaku = Get_syuuyakuten(sUser, conn2, sData[0], sData[1]);
					sRet[0] = sSyuyaku[0];
					if(sRet[0] != " ") return sRet;
					string s集約店ＣＤ = sSyuyaku[1];

// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 START
					//社内伝会員の場合、発店と集約店を自店所用に更新
					if(sData[0].Substring(0,2).ToUpper() == "FK")
					{
						// 発店、集約店決定
						string[] sHatuten_HouseSlip = Get_hatuten_HouseSlip(sUser, conn2, sData[0]);
						sRet[0] = sHatuten_HouseSlip[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s発店ＣＤ   = (sHatuten_HouseSlip[1].Length > 0) ? sHatuten_HouseSlip[1] : " ";
						s発店名     = (sHatuten_HouseSlip[2].Length > 0) ? sHatuten_HouseSlip[2] : " ";
						s集約店ＣＤ = (sHatuten_HouseSlip[3].Length > 0) ? sHatuten_HouseSlip[3] : " ";
					}
// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 END

// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 START
					//仕分ＣＤ取得
					string s仕分ＣＤ = " ";
					if(s発店ＣＤ.Trim().Length > 0 && s着店ＣＤ.Trim().Length > 0){
						string[] sRetSiwake = Get_siwake(sUser, conn2, s発店ＣＤ, s着店ＣＤ);
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） END
//						if(sRet[0] != " ") return sRet;
						s仕分ＣＤ = sRetSiwake[1];
					}
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 END

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
// MOD 2011.07.14 東都）高木 記事行の追加 START
//					// 処理０２に才数および重量の参考値を入れる
//					string s才数 = "";
//					string s重量 = "";
//					string s才数重量 = "";
//					try{
//						s才数 = sData[38].Trim().PadLeft(5,'0');
//						s重量 = sData[20].Trim().PadLeft(5,'0');
//						s才数重量 = s才数.Substring(0,5)
//									+ s重量.Substring(0,5);
//					}catch(Exception){
//					}
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
///					string s重量入力制御 = (sData.Length > 42) ? sData[42] : "0";
///					if(s重量入力制御 != "1"){
///					string s重量入力制御 = (sData.Length > 42) ? sData[42] : " ";
					if(s重量入力制御 == "0"){
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						sData[38] = "0"; // 才数
						sData[20] = "0"; // 重量
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					string s品名記事４ = (sData.Length > 43) ? sData[43] : " ";
					string s品名記事５ = (sData.Length > 44) ? sData[44] : " ";
					string s品名記事６ = (sData.Length > 45) ? sData[45] : " ";
					if(s品名記事４.Length == 0) s品名記事４ = " ";
// MOD 2011.07.14 東都）高木 記事行の追加 END
					cmdQuery 
						= "INSERT INTO \"ＳＴ０１出荷ジャーナル\" \n"
// MOD 2010.10.13 東都）高木 [品名記事４]など項目追加 START
						+ "(会員ＣＤ, 部門ＣＤ, 登録日, \"ジャーナルＮＯ\", 出荷日 \n"
						+ ", お客様出荷番号, 荷受人ＣＤ \n"
						+ ", 電話番号１, 電話番号２, 電話番号３, ＦＡＸ番号１, ＦＡＸ番号２, ＦＡＸ番号３ \n"
						+ ", 住所ＣＤ, 住所１, 住所２, 住所３ \n"
						+ ", 名前１, 名前２, 名前３ \n"
						+ ", 郵便番号 \n"
						+ ", 着店ＣＤ, 着店名, 特殊計 \n"
						+ ", 荷送人ＣＤ, 荷送人部署名 \n"
						+ ", 集約店ＣＤ, 発店ＣＤ, 発店名 \n"
						+ ", 得意先ＣＤ, 部課ＣＤ, 部課名 \n"
						+ ", 個数, 才数, 重量, ユニット \n"
						+ ", 指定日, 指定日区分 \n"
						+ ", 輸送指示ＣＤ１, 輸送指示１ \n"
						+ ", 輸送指示ＣＤ２, 輸送指示２ \n"
						+ ", 品名記事１, 品名記事２, 品名記事３ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 START
						+ ", 品名記事４, 品名記事５, 品名記事６ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
						+ ", 元着区分, 保険金額, 運賃, 中継, 諸料金 \n"
						+ ", 仕分ＣＤ, 送り状番号, 送り状区分 \n"
						+ ", 送り状発行済ＦＧ, 出荷済ＦＧ, 送信済ＦＧ, 一括出荷ＦＧ \n"
						+ ", 状態, 詳細状態 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
						+ ", 処理０２ \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷登録時は、処理０４に「0」を設定する
						+ ",処理０４ \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
						+ ", 削除ＦＧ, 登録日時, 登録ＰＧ, 登録者 \n"
						+ ", 更新日時, 更新ＰＧ, 更新者 \n"
						+ ") \n"
// MOD 2010.10.13 東都）高木 [品名記事４]など項目追加 END
						+ "VALUES ('" + sData[0] +"','" + sData[1] +"','" + s日付 +"'," + i管理ＮＯ +",'" + sData[2] +"', \n"
						+         "'" + sData[3] +"','" + sData[4] +"', \n"
						+         "'" + sData[5] +"','" + sData[6] +"','" + sData[7] +"',' ',' ',' ', \n"
						+         "'" + s住所ＣＤ +"','" + sData[8] +"','" + sData[9] +"','" + sData[10] +"', \n"
						+         "'" + sData[11] +"','" + sData[12] +"',' ', \n"
						+         "'" + sData[13] + sData[14] +"', \n"
						+         "'" + s着店ＣＤ +"','" + s着店名 + "','" + s特殊計 +"', \n"        //着店ＣＤ　着店名　特殊計
						+         "'" + sData[15] +"','" + sData[37] +"', \n"						  // 荷送人ＣＤ  荷送人部署名
						+         "'" + s集約店ＣＤ + "','" + s発店ＣＤ + "','" + s発店名 + "', \n"  //集約店ＣＤ　発店ＣＤ　発店名
						+         "'" + sData[16] +"','" + sData[17] +"','" + sData[18] +"', \n"
						+         "" + sData[19] +"," + sData[38] +"," + sData[20] +",0, \n"
// MOD 2005.06.08 東都）伊賀 指定日区分追加 START
// MOD 2005.06.01 東都）伊賀 輸送商品コード追加 START
//						+         "'" + sData[21] +"','" + sData[22] +"','" + sData[23] +"', \n"
//						+         "'" + sData[21] +"','" + sData[39] +"','" + sData[22] +"','" + sData[40] +"','" + sData[23] +"', \n"
						+         "'" + sData[21] +"','" + sData[41] +"', \n"
						+         "'" + sData[39] +"','" + sData[22] +"', \n"
						+         "'" + sData[40] +"','" + sData[23] +"', \n"
// MOD 2005.06.01 東都）伊賀 輸送商品コード追加 END
// MOD 2005.06.08 東都）伊賀 指定日区分追加 END
						+         "'" + sData[24] +"','" + sData[25] +"','" + sData[26] +"', \n"
// MOD 2011.07.14 東都）高木 記事行の追加 START
						+         "'" + s品名記事４ +"','"+ s品名記事５ +"','"+ s品名記事６ +"', \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
						+         "'" + sData[27] +"'," + sData[28] +",0,0,0, \n"  //運賃　中継　諸料金
// MOD 2007.02.08 東都）高木 仕分ＣＤの追加 START
//						+         "' ',' ',' ',"  //  仕分ＣＤ  送り状番号  送り状区分
						+         "'" + s仕分ＣＤ + "',' ',' ',"  //  仕分ＣＤ  送り状番号  送り状区分
// MOD 2007.02.08 東都）高木 仕分ＣＤの追加 END
						+         "'" + sData[29] +"','" + sData[30] +"', '0', '" + sData[31] +"', \n"  //   送信済ＦＧ
						+         "'01','  ', \n"        //状態　詳細状態
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
// MOD 2011.07.14 東都）高木 記事行の追加 START
//						+         "'" + s才数重量 + "', \n" // 処理０２
						+         "' ', \n" // 処理０２
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷登録時は、処理０４に「0」を設定する
						+         "'0', \n" // 処理０４
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
						+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[32] +"','" + sData[33] +"', \n"
						+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[32] +"','" + sData[33] +"')";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "出荷登録["+sData[1]+"]["+s日付+"]["+i管理ＮＯ+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

					// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
					//		MOD-S 2012.10.01 COA)横山 Oracleサーバ負荷軽減対策（ORA-01461により、ST01へのINSERTを元のコードに戻す）
					/**/
					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					/**/
					/*
					logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

					cmdQuery 
						= "INSERT INTO \"ＳＴ０１出荷ジャーナル\" \n"
						+ "(会員ＣＤ, 部門ＣＤ, 登録日, \"ジャーナルＮＯ\", 出荷日 \n"
						+ ", お客様出荷番号, 荷受人ＣＤ \n"
						+ ", 電話番号１, 電話番号２, 電話番号３, ＦＡＸ番号１, ＦＡＸ番号２, ＦＡＸ番号３ \n"
						+ ", 住所ＣＤ, 住所１, 住所２, 住所３ \n"
						+ ", 名前１, 名前２, 名前３ \n"
						+ ", 郵便番号 \n"
						+ ", 着店ＣＤ, 着店名, 特殊計 \n"
						+ ", 荷送人ＣＤ, 荷送人部署名 \n"
						+ ", 集約店ＣＤ, 発店ＣＤ, 発店名 \n"
						+ ", 得意先ＣＤ, 部課ＣＤ, 部課名 \n"
						+ ", 個数, 才数, 重量, ユニット \n"
						+ ", 指定日, 指定日区分 \n"
						+ ", 輸送指示ＣＤ１, 輸送指示１ \n"
						+ ", 輸送指示ＣＤ２, 輸送指示２ \n"
						+ ", 品名記事１, 品名記事２, 品名記事３ \n"
						+ ", 品名記事４, 品名記事５, 品名記事６ \n"
						+ ", 元着区分, 保険金額, 運賃, 中継, 諸料金 \n"
						+ ", 仕分ＣＤ, 送り状番号, 送り状区分 \n"
						+ ", 送り状発行済ＦＧ, 出荷済ＦＧ, 送信済ＦＧ, 一括出荷ＦＧ \n"
						+ ", 状態, 詳細状態 \n"
						+ ", 処理０２ \n"
						+ ", 削除ＦＧ, 登録日時, 登録ＰＧ, 登録者 \n"
						+ ", 更新日時, 更新ＰＧ, 更新者 \n"
						+ ") \n"
						+ "VALUES (:p_KaiinCD, :p_BumonCD, :p_TourokuBi, :p_JournalNo, :p_SyukkaBi, \n"
						+         ":p_CstmSyukkaNo, :p_NiukeCD, \n"
						+         ":p_TelNo_1, :p_TelNo_2, :p_TelNo_3, ' ', ' ', ' ', \n"
						+         ":p_AddrCD, :p_Addr_1, :p_Addr_2, :p_Addr_3, \n"
						+         ":p_Name_1, :p_Name_2, ' ', \n"
						+         ":p_YuubinNo, \n"
						+         ":p_ChakutenCD, :p_ChakutenName, :p_TokushuKei, \n"		//着店ＣＤ　着店名　特殊計
						+         ":p_NiokuriCD, :p_NiokuriBusho, \n"						// 荷送人ＣＤ  荷送人部署名
						+         ":p_ShuuyakutenCD, :p_HatsutenCD, :p_HatsutenName, \n"	//集約店ＣＤ　発店ＣＤ　発店名
						+         ":p_TokuiCD, :p_TokuiBukaCD, :p_TokuiBukaName, \n"
						+         ":p_Kosuu, :p_Saisuu, :p_Juuryou, 0, \n"
						+         ":p_ShiteiBi, :p_ShiteiBiKBN, \n"
						+         ":p_YusoushijiCD_1, :p_Yusoushiji_1, \n"
						+         ":p_YusoushijiCD_2, :p_Yusoushiji_2, \n"
						+         ":p_Kiji_1, :p_Kiji_2, :p_Kiji_3, \n"
						+         ":p_Kiji_4, :p_Kiji_5, :p_Kiji_6, \n"
						+         ":p_MotoChakuKBN, :p_Hoken, 0, 0, 0, \n"						//運賃　中継　諸料金
						+         ":p_ShiwakeCD, ' ', ' ', "									//  仕分ＣＤ  送り状番号  送り状区分
						+         ":p_HakkouSumiFG, :p_SyukkaSumiFG, '0', :p_IsseiSyukkaFG, \n"	//   送信済ＦＧ
						+         "'01','  ', \n"        //状態　詳細状態
						+         "' ', \n" // 処理０２
						+         "'0', TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), :p_TourokuPG, :p_Tourokusha, \n"
						+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), :p_KoushinPG, :p_Koushinsha)";

					wk_opOraParam	= new OracleParameter[53];
					wk_opOraParam[0]  = new OracleParameter("p_KaiinCD",        OracleDbType.Char,    sData[0],    ParameterDirection.Input);
					wk_opOraParam[1]  = new OracleParameter("p_BumonCD",        OracleDbType.Char,    sData[1],    ParameterDirection.Input);
					wk_opOraParam[2]  = new OracleParameter("p_TourokuBi",      OracleDbType.Char,    s日付,       ParameterDirection.Input);
					wk_opOraParam[3]  = new OracleParameter("p_JournalNo",      OracleDbType.Decimal, i管理ＮＯ,   ParameterDirection.Input);
					wk_opOraParam[4]  = new OracleParameter("p_SyukkaBi",       OracleDbType.Char,    sData[2],    ParameterDirection.Input);
					wk_opOraParam[5]  = new OracleParameter("p_CstmSyukkaNo",   OracleDbType.Char,    sData[3],    ParameterDirection.Input);
					wk_opOraParam[6]  = new OracleParameter("p_NiukeCD",        OracleDbType.Char,    sData[4],    ParameterDirection.Input);
					wk_opOraParam[7]  = new OracleParameter("p_TelNo_1",        OracleDbType.Char,    sData[5],    ParameterDirection.Input);
					wk_opOraParam[8]  = new OracleParameter("p_TelNo_2",        OracleDbType.Char,    sData[6],    ParameterDirection.Input);
					wk_opOraParam[9]  = new OracleParameter("p_TelNo_3",        OracleDbType.Char,    sData[7],    ParameterDirection.Input);
					wk_opOraParam[10] = new OracleParameter("p_AddrCD",         OracleDbType.Char,    s住所ＣＤ,   ParameterDirection.Input);
					wk_opOraParam[11] = new OracleParameter("p_Addr_1",         OracleDbType.Char,    sData[8],    ParameterDirection.Input);
					wk_opOraParam[12] = new OracleParameter("p_Addr_2",         OracleDbType.Char,    sData[9],    ParameterDirection.Input);
					wk_opOraParam[13] = new OracleParameter("p_Addr_3",         OracleDbType.Char,    sData[10],   ParameterDirection.Input);
					wk_opOraParam[14] = new OracleParameter("p_Name_1",         OracleDbType.Char,    sData[11],   ParameterDirection.Input);
					wk_opOraParam[15] = new OracleParameter("p_Name_2",         OracleDbType.Char,    sData[12],   ParameterDirection.Input);
					wk_opOraParam[16] = new OracleParameter("p_YuubinNo",       OracleDbType.Char,    sData[13]+sData[14], ParameterDirection.Input);
					wk_opOraParam[17] = new OracleParameter("p_ChakutenCD",     OracleDbType.Char,    s着店ＣＤ,   ParameterDirection.Input);
					wk_opOraParam[18] = new OracleParameter("p_ChakutenName",   OracleDbType.Char,    s着店名,     ParameterDirection.Input);
					wk_opOraParam[19] = new OracleParameter("p_TokushuKei",     OracleDbType.Char,    s特殊計,     ParameterDirection.Input);
					wk_opOraParam[20] = new OracleParameter("p_NiokuriCD",      OracleDbType.Char,    sData[15],   ParameterDirection.Input);
					wk_opOraParam[21] = new OracleParameter("p_NiokuriBusho",   OracleDbType.Char,    sData[37],   ParameterDirection.Input);
					wk_opOraParam[22] = new OracleParameter("p_ShuuyakutenCD",  OracleDbType.Char,    s集約店ＣＤ, ParameterDirection.Input);
					wk_opOraParam[23] = new OracleParameter("p_HatsutenCD",     OracleDbType.Char,    s発店ＣＤ,   ParameterDirection.Input);
					wk_opOraParam[24] = new OracleParameter("p_HatsutenName",   OracleDbType.Char,    s発店名,     ParameterDirection.Input);
					wk_opOraParam[25] = new OracleParameter("p_TokuiCD",        OracleDbType.Char,    sData[16],   ParameterDirection.Input);
					wk_opOraParam[26] = new OracleParameter("p_TokuiBukaCD",    OracleDbType.Char,    sData[17],   ParameterDirection.Input);
					wk_opOraParam[27] = new OracleParameter("p_TokuiBukaName",  OracleDbType.Char,    sData[18],   ParameterDirection.Input);
					wk_opOraParam[28] = new OracleParameter("p_Kosuu",          OracleDbType.Decimal, sData[19],   ParameterDirection.Input);
					wk_opOraParam[29] = new OracleParameter("p_Saisuu",         OracleDbType.Decimal, sData[38],   ParameterDirection.Input);
					wk_opOraParam[30] = new OracleParameter("p_Juuryou",        OracleDbType.Decimal, sData[20],   ParameterDirection.Input);
					wk_opOraParam[31] = new OracleParameter("p_ShiteiBi",       OracleDbType.Char,    sData[21],   ParameterDirection.Input);
					wk_opOraParam[32] = new OracleParameter("p_ShiteiBiKBN",    OracleDbType.Char,    sData[41],   ParameterDirection.Input);
					wk_opOraParam[33] = new OracleParameter("p_YusoushijiCD_1", OracleDbType.Char,    sData[39],   ParameterDirection.Input);
					wk_opOraParam[34] = new OracleParameter("p_Yusoushiji_1",   OracleDbType.Char,    sData[22],   ParameterDirection.Input);
					wk_opOraParam[35] = new OracleParameter("p_YusoushijiCD_2", OracleDbType.Char,    sData[40],   ParameterDirection.Input);
					wk_opOraParam[36] = new OracleParameter("p_Yusoushiji_2",   OracleDbType.Char,    sData[23],   ParameterDirection.Input);
					wk_opOraParam[37] = new OracleParameter("p_Kiji_1",         OracleDbType.Char,    sData[24],   ParameterDirection.Input);
					wk_opOraParam[38] = new OracleParameter("p_Kiji_2",         OracleDbType.Char,    sData[25],   ParameterDirection.Input);
					wk_opOraParam[39] = new OracleParameter("p_Kiji_3",         OracleDbType.Char,    sData[26],   ParameterDirection.Input);
					wk_opOraParam[40] = new OracleParameter("p_Kiji_4",         OracleDbType.Char,    s品名記事４, ParameterDirection.Input);
					wk_opOraParam[41] = new OracleParameter("p_Kiji_5",         OracleDbType.Char,    s品名記事５, ParameterDirection.Input);
					wk_opOraParam[42] = new OracleParameter("p_Kiji_6",         OracleDbType.Char,    s品名記事６, ParameterDirection.Input);
					wk_opOraParam[43] = new OracleParameter("p_MotoChakuKBN",   OracleDbType.Char,    sData[27],   ParameterDirection.Input);
					wk_opOraParam[44] = new OracleParameter("p_Hoken",          OracleDbType.Decimal, sData[28],   ParameterDirection.Input);
					wk_opOraParam[45] = new OracleParameter("p_ShiwakeCD",      OracleDbType.Char,    s仕分ＣＤ,   ParameterDirection.Input);
					wk_opOraParam[46] = new OracleParameter("p_HakkouSumiFG",   OracleDbType.Char,    sData[29],   ParameterDirection.Input);
					wk_opOraParam[47] = new OracleParameter("p_SyukkaSumiFG",   OracleDbType.Char,    sData[30],   ParameterDirection.Input);
					wk_opOraParam[48] = new OracleParameter("p_IsseiSyukkaFG",  OracleDbType.Char,    sData[31],   ParameterDirection.Input);
					wk_opOraParam[49] = new OracleParameter("p_TourokuPG",      OracleDbType.Char,    sData[32],   ParameterDirection.Input);
					wk_opOraParam[50] = new OracleParameter("p_Tourokusha",     OracleDbType.Char,    sData[33],   ParameterDirection.Input);
					wk_opOraParam[51] = new OracleParameter("p_KoushinPG",      OracleDbType.Char,    sData[32],   ParameterDirection.Input);
					wk_opOraParam[52] = new OracleParameter("p_Koushinsha",     OracleDbType.Char,    sData[33],   ParameterDirection.Input);

					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					*/
					//		MOD-E 2012.10.01 COA)横山 Oracleサーバ負荷軽減対策（ORA-01461により、ST01へのINSERTを元のコードに戻す）
					// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

					tran.Commit();
					sRet[0] = "正常終了";
					sRet[1] = s日付;
					sRet[2] = i管理ＮＯ.ToString();
				}

			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 START
				if(ex.Number == 1438){ // ORA-01438: value larger than specified precision allows for this column
//					if(i管理ＮＯ > 9999){
						sRet[0] = "１日で扱える出荷数（9999件）を越えました。";
//					}
				}
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 END
			}
			catch (Exception ex)
			{
				tran.Rollback();
				string sErr = ex.Message.Substring(0,9);
				if(sErr == "ORA-00001")
					sRet[0] = "同一のコードが既に他の端末より登録されています。\r\n再度、最新データを呼び出して更新してください。";
				else
					sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 出荷データ更新
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_syukka(string[] sUser, string[] sData)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
		{
			return Upd_syukka2(sUser, sData, "-");
		}
		[WebMethod]
		public String[] Upd_syukka2(string[] sUser, string[] sData, string sNo)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷更新開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			decimal d件数;
			string s特殊計 = " ";
			try
			{
				//出荷日チェック
				string[] sSyukkabi = Get_bumonsyukka(sUser, conn2, sData[0], sData[1]);
				sRet[0] = sSyukkabi[0];
				if(sRet[0] != " ") return sRet;
				if(int.Parse(sData[2]) < int.Parse(sSyukkabi[1]))
				{
					sRet[0] = "1";
					sRet[1] = sSyukkabi[1];
					return sRet;
				}

				//荷送人ＣＤ存在チェック
				string cmdQuery
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
////					= "SELECT NVL(COUNT(*),0) \n"
////					= "SELECT COUNT(*) \n"
//// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
////					= "SELECT COUNT(ROWID) \n"
//					= "SELECT 得意先ＣＤ, 得意先部課ＣＤ \n"
//// MOD 2008.07.03 東都）高木 得意先情報の再取得 END
//					+ "  FROM ＳＭ０１荷送人 \n"
//					+ " WHERE 会員ＣＤ   = '" + sData[0]  +"' \n"
//					+ "   AND 部門ＣＤ   = '" + sData[1]  +"' \n"
//					+ "   AND 荷送人ＣＤ = '" + sData[15] +"' \n"
//					+ "   AND 削除ＦＧ   = '0'";
					= "SELECT SM01.得意先ＣＤ, SM01.得意先部課ＣＤ \n"
					+ "     , NVL(CM01.保留印刷ＦＧ,'0') \n"
					+ "  FROM ＳＭ０１荷送人 SM01 \n"
					+ "     , ＣＭ０１会員 CM01 \n"
					+ " WHERE SM01.会員ＣＤ   = '" + sData[0]  +"' \n"
					+ "   AND SM01.部門ＣＤ   = '" + sData[1]  +"' \n"
					+ "   AND SM01.荷送人ＣＤ = '" + sData[15] +"' \n"
					+ "   AND SM01.削除ＦＧ   = '0' \n"
					+ "   AND SM01.会員ＣＤ   = CM01.会員ＣＤ(+) \n"
					;
				string s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
//				reader.Read();
//				d件数   = reader.GetDecimal(0);
				if(reader.Read()){
					d件数 = 1;
					sData[16] = reader.GetString(0);
					sData[17] = reader.GetString(1);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(2);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				}else{
					d件数 = 0;
				}
// MOD 2008.07.03 東都）高木 得意先情報の再取得 END
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				if(d件数 == 0)
				{
					sRet[0] = "0";
				}
				else
				{
// MOD 2008.07.03 東都）高木 得意先情報の再取得 START
					cmdQuery
						= "SELECT SM04.得意先部課名 \n"
						+ " FROM ＣＭ０２部門 CM02 \n"
						+    " , ＳＭ０４請求先 SM04 \n"
						+ " WHERE CM02.会員ＣＤ = '" + sData[0] + "' \n"
						+  " AND CM02.部門ＣＤ = '" + sData[1] + "' \n"
						+  " AND CM02.削除ＦＧ = '0' \n"
						+  " AND SM04.会員ＣＤ = CM02.会員ＣＤ \n"
						+  " AND SM04.郵便番号 = CM02.郵便番号 \n"
						+  " AND SM04.得意先ＣＤ = '" + sData[16] + "' \n"
						+  " AND SM04.得意先部課ＣＤ = '" + sData[17] + "' \n"
						+  " AND SM04.削除ＦＧ = '0' \n"
						;
					reader = CmdSelect(sUser, conn2, cmdQuery);
					if(reader.Read()){
						sData[18] = reader.GetString(0);
					}else{
						sData[18] = " ";
					}
					disposeReader(reader);
					reader = null;
// MOD 2008.07.03 東都）高木 得意先情報の再取得 END

					//特殊計取得
					if(sData[4] != " ")
					{
						cmdQuery
							= "SELECT NVL(特殊計,' ') \n"
							+ "  FROM ＳＭ０２荷受人 \n"
							+ " WHERE 会員ＣＤ   = '" + sData[0] +"' \n"
							+ "   AND 部門ＣＤ   = '" + sData[1] +"' \n"
							+ "   AND 荷受人ＣＤ = '" + sData[4] +"' \n"
							+ "   AND 削除ＦＧ   = '0'";

						reader = CmdSelect(sUser, conn2, cmdQuery);

						bool bRead = reader.Read();
						if(bRead == true)
							s特殊計   = reader.GetString(0);

// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 START
						cmdQuery
							= "UPDATE ＳＭ０２荷受人 \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 START
//							+ " SET 名前３ = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET 登録ＰＧ = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 END
							+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
							+ " AND 部門ＣＤ   = '" + sData[1] +"' \n"
							+ " AND 荷受人ＣＤ = '" + sData[4] +"' \n"
							+ " AND 削除ＦＧ   = '0'";
						try{
							int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
						}catch(Exception){
							;
						}
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 END
					}

					//着店取得
// MOD 2008.12.25 kcl)森本 着店コード検索方法の再変更 START
//// MOD 2008.06.12 kcl)森本 着店コード検索方法の変更 START
////					string[] sTyakuten = Get_tyakuten(sUser, conn2, sData[13] + sData[14]);
//					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
//						sData[0], sData[1], sData[4], 
//						sData[13] + sData[14], sData[8] + sData[9] + sData[10]);
//// MOD 2008.06.12 kcl)森本 着店コード検索方法の変更 END
					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
						sData[0], sData[1], sData[4], 
						sData[13] + sData[14], sData[8] + sData[9] + sData[10], sData[11] + sData[12]);
// MOD 2008.12.25 kcl)森本 着店コード検索方法の再変更 END
					sRet[0] = sTyakuten[0];
					if(sRet[0] != " ") return sRet;
// MOD 2008.10.16 kcl)森本 着店コード等が Empty にならないようにする START
//					string s着店ＣＤ = sTyakuten[1];
//					string s着店名   = sTyakuten[2];
//					string s住所ＣＤ = sTyakuten[3];
					string s着店ＣＤ = (sTyakuten[1].Length > 0) ? sTyakuten[1] : " ";
					string s着店名   = (sTyakuten[2].Length > 0) ? sTyakuten[2] : " ";
					string s住所ＣＤ = (sTyakuten[3].Length > 0) ? sTyakuten[3] : " ";
// MOD 2008.10.16 kcl)森本 着店コード等が Empty にならないようにする END
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 START
					if(sData[8].Equals("※※支店止め※※"))
					{
						// 着店コード変換
						string s変換後着店ＣＤ = 半角数字変換(sData[10]);
						sRet[0] = s変換後着店ＣＤ;
						if(sRet[0].Length != 3)
						{
							// 半角変換失敗
							return sRet;
						}

						// 着店決定
						string[] sTyakuten_GeneralDelivery = Get_tyakuten_GeneralDelivery(sUser, conn2, s変換後着店ＣＤ, sData[13] + sData[14]);
						sRet[0] = sTyakuten_GeneralDelivery[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s着店ＣＤ = s変換後着店ＣＤ;
						s着店名   = (sTyakuten_GeneralDelivery[1].Length > 0) ? sTyakuten_GeneralDelivery[1] : " ";
					}
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 END

					//発店取得
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 START
//					string[] sHatuten = Get_hatuten(sData[15]);
//保留 MOD 2010.07.21 東都）高木 リコー様対応 START
					string[] sHatuten = Get_hatuten(sUser, conn2, sData[0], sData[1]);
//					string[] sHatuten = Get_hatuten3(sUser, conn2, sData[0], sData[1], sData[15]);
//保留 MOD 2010.07.21 東都）高木 リコー様対応 END
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 END
					sRet[0] = sHatuten[0];
					if(sRet[0] != " ") return sRet;
					string s発店ＣＤ = sHatuten[1];
					string s発店名   = sHatuten[2];

					//集荷店取得
					string[] sSyuyaku = Get_syuuyakuten(sUser, conn2, sData[0], sData[1]);
					sRet[0] = sSyuyaku[0];
					if(sRet[0] != " ") return sRet;
					string s集約店ＣＤ = sSyuyaku[1];

// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 START
					//社内伝会員の場合、発店と集約店を自店所用に更新
					if(sData[0].Substring(0,2).ToUpper() == "FK")
					{
						// 発店、集約店決定
						string[] sHatuten_HouseSlip = Get_hatuten_HouseSlip(sUser, conn2, sData[0]);
						sRet[0] = sHatuten_HouseSlip[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s発店ＣＤ   = (sHatuten_HouseSlip[1].Length > 0) ? sHatuten_HouseSlip[1] : " ";
						s発店名     = (sHatuten_HouseSlip[2].Length > 0) ? sHatuten_HouseSlip[2] : " ";
						s集約店ＣＤ = (sHatuten_HouseSlip[3].Length > 0) ? sHatuten_HouseSlip[3] : " ";
					}
// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 END

// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 START
					//仕分ＣＤ取得
					string s仕分ＣＤ = " ";
					if(s発店ＣＤ.Trim().Length > 0 && s着店ＣＤ.Trim().Length > 0){
						string[] sRetSiwake = Get_siwake(sUser, conn2, s発店ＣＤ, s着店ＣＤ);
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） END
//						if(sRet[0] != " ") return sRet;
						s仕分ＣＤ = sRetSiwake[1];
					}
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 END

// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
///					string s重量入力制御 = (sData.Length > 42) ? sData[42] : "0";
///					if(s重量入力制御 != "1"){
///					string s重量入力制御 = (sData.Length > 42) ? sData[42] : " ";
					if(s重量入力制御 == "0"){
						sData[38] = "0"; // 才数
						sData[20] = "0"; // 重量
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					string s品名記事４ = (sData.Length > 43) ? sData[43] : " ";
					string s品名記事５ = (sData.Length > 44) ? sData[44] : " ";
					string s品名記事６ = (sData.Length > 45) ? sData[45] : " ";
					if(s品名記事４.Length == 0) s品名記事４ = " ";
// MOD 2011.07.14 東都）高木 記事行の追加 END

					cmdQuery 
						= "UPDATE \"ＳＴ０１出荷ジャーナル\" \n"
						+    "SET 出荷日             = '" + sData[2]  +"', \n"
						+        "お客様出荷番号     = '" + sData[3]  +"',"
						+        "荷受人ＣＤ         = '" + sData[4]  +"',"
						+        "電話番号１         = '" + sData[5]  +"', \n"
						+        "電話番号２         = '" + sData[6]  +"',"
						+        "電話番号３         = '" + sData[7]  +"',"
						+        "住所ＣＤ           = '" + s住所ＣＤ +"', \n"
						+        "住所１             = '" + sData[8]  +"',"
						+        "住所２             = '" + sData[9]  +"',"
						+        "住所３             = '" + sData[10] +"', \n"
						+        "名前１             = '" + sData[11] +"',"
						+        "名前２             = '" + sData[12] +"',"
						+        "郵便番号           = '" + sData[13] + sData[14] +"', \n"
						+        "着店ＣＤ           = '" + s着店ＣＤ +"',"
						+        "着店名             = '" + s着店名   +"',"
						+        "特殊計             = '" + s特殊計   +"', \n"
						+        "荷送人ＣＤ         = '" + sData[15] +"',"
						+        "荷送人部署名       = '" + sData[37] +"',"
						+        "集約店ＣＤ         = '" + s集約店ＣＤ +"', \n"
						+        "発店ＣＤ           = '" + s発店ＣＤ +"',"
						+        "発店名             = '" + s発店名   +"',"
						+        "得意先ＣＤ         = '" + sData[16] +"', \n"
						+        "部課ＣＤ           = '" + sData[17] +"',"
						+        "部課名             = '" + sData[18] +"',"
						+        "個数               =  " + sData[19] +", \n"
// MOD 2005.05.17 東都）小童谷 才数追加 START
						+        "才数               =  " + sData[38] +","
// MOD 2005.05.17 東都）小童谷 才数追加 END
						+        "重量               =  " + sData[20] +","
						+        "指定日             = '" + sData[21] +"',"
// ADD 2005.06.01 東都）伊賀 指定日区分追加 START
						+        "指定日区分         = '" + sData[41] +"',"
// ADD 2005.06.01 東都）伊賀 指定日区分追加 END
// ADD 2005.06.01 東都）伊賀 輸送商品コード追加 START
						+        "輸送指示ＣＤ１     = '" + sData[39] +"',"
// ADD 2005.06.01 東都）伊賀 輸送商品コード追加 END
						+        "輸送指示１         = '" + sData[22] +"', \n"
// ADD 2005.06.01 東都）伊賀 輸送商品コード追加 START
						+        "輸送指示ＣＤ２     = '" + sData[40] +"',"
// ADD 2005.06.01 東都）伊賀 輸送商品コード追加 END
						+        "輸送指示２         = '" + sData[23] +"',"
						+        "品名記事１         = '" + sData[24] +"',"
						+        "品名記事２         = '" + sData[25] +"', \n"
						+        "品名記事３         = '" + sData[26] +"',"
// MOD 2011.07.14 東都）高木 記事行の追加 START
						+        "品名記事４         = '" + s品名記事４ +"', \n"
						+        "品名記事５         = '" + s品名記事５ +"',"
						+        "品名記事６         = '" + s品名記事６ +"', \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
						+        "保険金額           =  " + sData[28] +","
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 START
						+        "仕分ＣＤ           = '" + s仕分ＣＤ + "', \n"
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 END
						+        "送り状発行済ＦＧ   = '0', \n"
						+        "送信済ＦＧ         = '0',"
//						+        "状態               = DECODE(状態,'03','02','01'),"
						+        "状態               = '01',"
						+        "詳細状態           = '  ', \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷更新時（削除時も同様）は、処理０４に「1」を設定する
						+        "処理０４           = '1',"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
						+        "更新ＰＧ           = '" + sData[32] +"',"
						+        "更新者             = '" + sData[33] +"', \n"
						+        "更新日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE 会員ＣＤ           = '" + sData[0]  +"' \n"
						+ "   AND 部門ＣＤ           = '" + sData[1]  +"' \n"
						+ "   AND 登録日             = '" + sData[35] +"' \n"
						+ "   AND \"ジャーナルＮＯ\" = '" + sData[34] +"' \n"
						+ "   AND 更新日時           =  " + sData[36] +"";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "出荷更新["+sData[1]+"]["+sData[35]+"]["+sData[34]+"]:["+sNo+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					tran.Commit();
					if(iUpdRow == 0)
						sRet[0] = "データ編集中に他の端末より更新されています。\r\n再度、最新データを呼び出して更新してください。";
					else
						sRet[0] = "正常終了";
				}
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 出荷データ削除
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ、更新ＰＧ、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_syukka(string[] sUser, string[] sData)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
		{
			return Del_syukka2(sUser, sData, "-");
		}
		[WebMethod]
		public String[] Del_syukka2(string[] sUser, string[] sData, string sNo)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷削除開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "UPDATE \"ＳＴ０１出荷ジャーナル\" \n"
					+    "SET 送信済ＦＧ         = '0', \n"
					+       " 削除ＦＧ           = '1', \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
					//出荷削除時（更新時も同様）は、処理０４に「1」を設定する
					+        "処理０４           = '1', \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
// MOD 2010.10.27 東都）高木 削除日時などの追加 START
//// MOD 2009.09.11 東都）高木 出荷照会で出荷済ＦＧ,送信済ＦＧなどを追加 START
//					+        "登録日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
////					+        "更新ＰＧ           = '" + sData[4] +"', \n"
////					+        "更新者             = '" + sData[5] +"', \n"
////					+        "更新日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
//// MOD 2009.09.11 東都）高木 出荷照会で出荷済ＦＧ,送信済ＦＧなどを追加 END
					+        "削除ＰＧ           = '" + sData[4] +"', \n"
					+        "削除者             = '" + sData[5] +"', \n"
					+        "削除日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
// MOD 2010.10.27 東都）高木 削除日時などの追加 END
					+ " WHERE 会員ＣＤ           = '" + sData[0] +"' \n"
					+ "   AND 部門ＣＤ           = '" + sData[1] +"' \n"
					+ "   AND 登録日             = '" + sData[2] +"' \n"
					+ "   AND \"ジャーナルＮＯ\" = '" + sData[3] +"'";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "出荷削除["+sData[1]+"]["+sData[2]+"]["+sData[3]+"]:["+sNo+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//保留　削除なので排他チェックしない（削除優先）
				tran.Commit();				
				sRet[0] = "正常終了";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 出荷データ一括削除
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ、更新ＰＧ、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_ikkatu(string[] sUser, string[] sData, string[] sInday, string[] sNo)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
		{
			string[] sONo = new string[sInday.Length];
			for(int iCnt = 0; iCnt < sONo.Length; iCnt++){
				sONo[iCnt] = "-";
			}
			return Del_ikkatu2(sUser, sData, sInday, sNo, sONo);
		}
		[WebMethod]
		public String[] Del_ikkatu2(string[] sUser, string[] sData
									, string[] sInday, string[] sNo, string[] sONo)
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷一括削除開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery; 
				for(int iCnt = 0; iCnt < sInday.Length && sNo[iCnt] != ""; iCnt++)
				{
					cmdQuery
						= "UPDATE \"ＳＴ０１出荷ジャーナル\" \n"
						+    "SET 送信済ＦＧ         = '0', \n"
						+       " 削除ＦＧ           = '1', \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷削除時（更新時も同様）は、処理０４に「1」を設定する
						+        "処理０４           = '1', \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
// MOD 2010.10.27 東都）高木 削除日時などの追加 START
//// MOD 2009.09.11 東都）高木 出荷照会で出荷済ＦＧ,送信済ＦＧなどを追加 START
//						+        "登録日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
////						+        "更新ＰＧ           = '" + sData[2] +"', \n"
////						+        "更新者             = '" + sData[3] +"', \n"
////						+        "更新日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
//// MOD 2009.09.11 東都）高木 出荷照会で出荷済ＦＧ,送信済ＦＧなどを追加 END
						+        "削除ＰＧ           = '" + sData[2] +"', \n"
						+        "削除者             = '" + sData[3] +"', \n"
						+        "削除日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
// MOD 2010.10.27 東都）高木 削除日時などの追加 END
						+ " WHERE 会員ＣＤ           = '" + sData[0] +"' \n"
						+ "   AND 部門ＣＤ           = '" + sData[1] +"' \n"
						+ "   AND 登録日             = '" + sInday[iCnt] +"' \n"
						+ "   AND \"ジャーナルＮＯ\" = '" + sNo[iCnt] +"'";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "出荷一括削除["+sData[1]+"]["+sInday[iCnt]+"]["+sNo[iCnt]+"]:["+sONo[iCnt]+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//保留　削除なので排他チェックしない（削除優先）
				}
				tran.Commit();				
				sRet[0] = "正常終了";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 着店取得
		 * 引数：郵便番号
		 * 戻値：ステータス、店所ＣＤ、店所名、都道府県ＣＤ、市区町村ＣＤ、大字通称ＣＤ
		 *
		 *********************************************************************/
		private String[] Get_tyakuten(string[] sUser, OracleConnection conn2, string sYuubin)
		{
			string[] sRet = new string[4];

/*			string cmdQuery = "SELECT T.店所ＣＤ,T.店所名 "
				+ " FROM ＣＭ１４郵便番号 Y,ＣＭ１３住所 J,ＣＭ１０店所 T"
				+ " WHERE Y.郵便番号 = '" + sYuubin + "'"
				+ "   AND Y.都道府県ＣＤ = J.都道府県ＣＤ"
				+ "   AND Y.市区町村ＣＤ = J.市区町村ＣＤ"
				+ "   AND Y.大字通称ＣＤ = J.大字通称ＣＤ"
				+ "   AND Y.削除ＦＧ     = '0'"
				+ "   AND J.店所ＣＤ     = T.店所ＣＤ"
				+ "   AND J.削除ＦＧ     = '0'"
				+ "   AND T.削除ＦＧ     = '0'";
*/
			string cmdQuery
				= "SELECT T.店所ＣＤ,T.店所名,Y.都道府県ＣＤ || Y.市区町村ＣＤ || Y.大字通称ＣＤ \n"
				+ " FROM ＣＭ１４郵便番号 Y,ＣＭ１０店所 T \n"
				+ " WHERE Y.郵便番号 = '" + sYuubin + "' \n"
				+ "   AND Y.削除ＦＧ     = '0' \n"
				+ "   AND Y.店所ＣＤ     = T.店所ＣＤ \n"
				+ "   AND T.削除ＦＧ     = '0'";

			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

			bool bRead = reader.Read();
			if(bRead == true)
			{
				for(int iCnt = 1; iCnt < 4; iCnt++)
				{
					sRet[iCnt] = reader.GetString(iCnt - 1).Trim();
				}
				sRet[0] = " ";
			}
			else
			{
				sRet[0] = "入力されたお届け先(郵便番号)では配達店が決められませんでした";
				sRet[1] = "0000";
				sRet[2] = " ";
				sRet[3] = " ";
			}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			
			return sRet;
		}

		/*********************************************************************
		 * 着店取得
		 * 引数：郵便番号
		 * 戻値：ステータス、店所ＣＤ、店所名、都道府県ＣＤ、市区町村ＣＤ、大字通称ＣＤ
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_tyakuten2(string[] sUser, string sYuubin)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "着店取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery
					= "SELECT T.店所ＣＤ,T.店所名,Y.都道府県ＣＤ || Y.市区町村ＣＤ || Y.大字通称ＣＤ \n"
					+ " FROM ＣＭ１４郵便番号 Y,ＣＭ１０店所 T \n"
					+ " WHERE Y.郵便番号 = '" + sYuubin + "' \n"
					+ "   AND Y.削除ＦＧ     = '0' \n"
					+ "   AND Y.店所ＣＤ     = T.店所ＣＤ \n"
					+ "   AND T.削除ＦＧ     = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					for(int iCnt = 1; iCnt < 4; iCnt++)
					{
						sRet[iCnt] = reader.GetString(iCnt - 1).Trim();
					}
					sRet[0] = "正常終了";
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 発店取得
		 * 引数：荷送人ＣＤ
		 * 戻値：ステータス、店所ＣＤ、店所名、都道府県ＣＤ、市区町村ＣＤ、大字通称ＣＤ
		 *
		 *********************************************************************/
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 START
//		private String[] Get_hatuten(string sIcode)
		private String[] Get_hatuten(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 END
		{
			string[] sRet = new string[4];
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）


/*			string cmdQuery = "SELECT T.店所ＣＤ,T.店所名 "
				+ " FROM ＳＭ０１荷送人 N,ＣＭ１４郵便番号 Y,ＣＭ１３住所 J,ＣＭ１０店所 T"
				+ " WHERE N.荷送人ＣＤ   = '" + sIcode + "'"
				+ "   AND N.郵便番号     = Y.郵便番号"
				+ "   AND N.削除ＦＧ     = '0'"
				+ "   AND Y.都道府県ＣＤ = J.都道府県ＣＤ"
				+ "   AND Y.市区町村ＣＤ = J.市区町村ＣＤ"
				+ "   AND Y.大字通称ＣＤ = J.大字通称ＣＤ"
				+ "   AND Y.削除ＦＧ     = '0'"
				+ "   AND J.店所ＣＤ     = T.店所ＣＤ"
				+ "   AND J.削除ＦＧ     = '0'"
				+ "   AND T.削除ＦＧ     = '0'";
*/
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 START
//			string cmdQuery
//				= "SELECT T.店所ＣＤ,T.店所名,Y.都道府県ＣＤ || Y.市区町村ＣＤ || Y.大字通称ＣＤ \n"
//				+ " FROM ＳＭ０１荷送人 N,ＣＭ１４郵便番号 Y,ＣＭ１０店所 T \n"
//				+ " WHERE N.荷送人ＣＤ   = '" + sIcode + "' \n"
//				+ "   AND N.郵便番号     = Y.郵便番号 \n"
//				+ "   AND N.削除ＦＧ     = '0' \n"
//				+ "   AND Y.削除ＦＧ     = '0' \n"
//				+ "   AND Y.店所ＣＤ     = T.店所ＣＤ \n"
//				+ "   AND T.削除ＦＧ     = '0'";
			string cmdQuery = "SELECT Y.店所ＣＤ, T.店所名, Y.都道府県ＣＤ, Y.市区町村ＣＤ, Y.大字通称ＣＤ \n"
				+ " FROM ＣＭ０２部門 B, \n"
				+      " ＣＭ１４郵便番号 Y, \n"
				+      " ＣＭ１０店所 T \n"
				+ " WHERE B.会員ＣＤ = '" + sKcode + "' \n"
				+ " AND B.部門ＣＤ = '" + sBcode + "' \n"
				+ " AND B.削除ＦＧ = '0' \n"
				+ " AND B.郵便番号 = Y.郵便番号 \n"
				+ " AND Y.店所ＣＤ = T.店所ＣＤ \n"
				+ " AND T.削除ＦＧ = '0' \n";
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 END

			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery = "SELECT Y.店所ＣＤ, T.店所名, Y.都道府県ＣＤ, Y.市区町村ＣＤ, Y.大字通称ＣＤ \n"
				+ " FROM ＣＭ０２部門 B, \n"
				+      " ＣＭ１４郵便番号 Y, \n"
				+      " ＣＭ１０店所 T \n"
				+ " WHERE B.会員ＣＤ = :p_KaiinCD \n"
				+ " AND B.部門ＣＤ = :p_BumonCD \n"
				+ " AND B.削除ＦＧ = '0' \n"
				+ " AND B.郵便番号 = Y.郵便番号 \n"
				+ " AND Y.店所ＣＤ = T.店所ＣＤ \n"
				+ " AND T.削除ＦＧ = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			bool bRead = reader.Read();
			if(bRead == true)
			{
// MOD 2005.05.11 東都）高木 発店の取得方法の修正 START
//				for(int iCnt = 1; iCnt < 4; iCnt++)
//				{
//					sRet[iCnt] = reader.GetString(iCnt - 1).Trim();
//				}
				sRet[1] = reader.GetString(0).Trim(); // 店所ＣＤ
				sRet[2] = reader.GetString(1).Trim(); // 店所名
				sRet[3] = reader.GetString(2).Trim()  // 住所ＣＤ
						+ reader.GetString(3).Trim()
						+ reader.GetString(4).Trim();

// MOD 2005.05.11 東都）高木 発店の取得方法の修正 END
				sRet[0] = " ";
			}
			else
			{
				sRet[0] = "発店を決められませんでした";
				sRet[1] = "0000";
				sRet[2] = " ";
				sRet[3] = " ";
			}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			
			return sRet;
		}

		/*********************************************************************
		 * 発店取得
		 * 引数：荷送人ＣＤ
		 * 戻値：ステータス、店所ＣＤ、店所名、都道府県ＣＤ、市区町村ＣＤ、大字通称ＣＤ
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_hatuten2(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "発店取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT Y.店所ＣＤ, T.店所名, Y.都道府県ＣＤ, Y.市区町村ＣＤ, Y.大字通称ＣＤ \n"
					+ " FROM ＣＭ０２部門 B, \n"
					+      " ＣＭ１４郵便番号 Y, \n"
					+      " ＣＭ１０店所 T \n"
					+ " WHERE B.会員ＣＤ = '" + sKcode + "' \n"
					+ " AND B.部門ＣＤ = '" + sBcode + "' \n"
					+ " AND B.削除ＦＧ = '0' \n"
					+ " AND B.郵便番号 = Y.郵便番号 \n"
					+ " AND Y.店所ＣＤ = T.店所ＣＤ \n"
					+ " AND T.削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim()
						+ reader.GetString(3).Trim()
						+ reader.GetString(4).Trim();

					sRet[0] = "正常終了";
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
//保留 MOD 2010.07.21 東都）高木 リコー様対応 START
//		private String[] Get_hatuten3(string[] sUser, OracleConnection conn2
//										, string sKcode, string sBcode, string sIcode)
//		{
//			string[] sRet = new string[4];
//			sRet = Get_hatuten(sUser, conn2, sKcode, sBcode);
//			if(sRet[1] != "030") return sRet;
//
//			string cmdQuery
//				= "SELECT NVL(CM14.店所ＣＤ,' ') \n"
//				+ ", NVL(CM10.店所名,' ') \n"
//				+ ", NVL(CM14.都道府県ＣＤ,' ') \n"
//				+ ", NVL(CM14.市区町村ＣＤ,' ') \n"
//				+ ", NVL(CM14.大字通称ＣＤ,' ') \n"
//				+ "  FROM ＳＭ０１荷送人 SM01 \n"
//				+ ", ＣＭ１４郵便番号 CM14 \n"
//				+ ", ＣＭ１０店所 CM10 \n"
//				+ " WHERE SM01.会員ＣＤ   = '" + sKcode +"' \n"
//				+ "   AND SM01.部門ＣＤ   = '" + sBcode +"' \n"
//				+ "   AND SM01.荷送人ＣＤ = '" + sIcode +"' \n"
//				+ "   AND SM01.郵便番号   = CM14.郵便番号(+)"
//				+ "   AND CM14.店所ＣＤ   = CM10.店所ＣＤ(+)"
//				;
//
//			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
//			if(reader.Read()){
//				sRet[1] = reader.GetString(0).Trim(); // 店所ＣＤ
//				sRet[2] = reader.GetString(1).Trim(); // 店所名
//				sRet[3] = reader.GetString(2).Trim()  // 住所ＣＤ
//						+ reader.GetString(3).Trim()
//						+ reader.GetString(4).Trim();
//
//				sRet[0] = " ";
//			}else{
//				sRet[0] = "発店を決められませんでした";
//				sRet[1] = "0000";
//				sRet[2] = " ";
//				sRet[3] = " ";
//			}
//			disposeReader(reader);
//			reader = null;
//
//			return sRet;
//		}
//保留 MOD 2010.07.21 東都）高木 リコー様対応 END
		/*********************************************************************
		 * 集約店取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、集約店ＣＤ
		 *
		 *********************************************************************/
		private String[] Get_syuuyakuten(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
		{
			string[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			string cmdQuery = "SELECT T.集約店ＣＤ \n"
				+ " FROM ＣＭ０２部門 B,ＣＭ１０店所 T, \n"
				+        "ＣＭ１４郵便番号 Y  \n"
				+ " WHERE B.会員ＣＤ   = '" + sKcode + "' \n"
				+ "   AND B.部門ＣＤ   = '" + sBcode + "' \n"
				+ "   AND B.削除ＦＧ     = '0' \n"
				+    "AND B.郵便番号 = Y.郵便番号 \n"
				+    "AND Y.店所ＣＤ     = T.店所ＣＤ \n"
				+ "   AND T.削除ＦＧ     = '0'";

			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery = "SELECT T.集約店ＣＤ \n"
				+ " FROM ＣＭ０２部門 B,ＣＭ１０店所 T, \n"
				+        "ＣＭ１４郵便番号 Y  \n"
				+ " WHERE B.会員ＣＤ   = :p_KaiinCD \n"
				+ "   AND B.部門ＣＤ   = :p_BumonCD \n"
				+ "   AND B.削除ＦＧ     = '0' \n"
				+    "AND B.郵便番号 = Y.郵便番号 \n"
				+    "AND Y.店所ＣＤ     = T.店所ＣＤ \n"
				+ "   AND T.削除ＦＧ     = '0'";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			bool bRead = reader.Read();
			if(bRead == true)
			{
				sRet[0] = " ";
				sRet[1] = reader.GetString(0).Trim();
			}
			else
			{
				sRet[0] = "集約店を決められませんでした";
				sRet[1] = "0000";
			}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

			return sRet;
		}

		/*********************************************************************
		 * 集約店取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、集約店ＣＤ
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syuuyakuten2(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "集約店取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT T.集約店ＣＤ \n"
					+ " FROM ＣＭ０２部門 B,ＣＭ１０店所 T, \n"
					+        "ＣＭ１４郵便番号 Y  \n"
					+ " WHERE B.会員ＣＤ   = '" + sKcode + "' \n"
					+ "   AND B.部門ＣＤ   = '" + sBcode + "' \n"
					+ "   AND B.削除ＦＧ     = '0' \n"
					+    "AND B.郵便番号 = Y.郵便番号 \n"
					+    "AND Y.店所ＣＤ     = T.店所ＣＤ \n"
					+ "   AND T.削除ＦＧ     = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 START
		/*********************************************************************
		 * 仕分ＣＤ取得
		 * 引数：会員ＣＤ、部門ＣＤ、ＤＢ接続、発店、着店
		 * 戻値：ステータス、仕分ＣＤ
		 *
		 *********************************************************************/
		private static string GET_SIWAKE_SELECT
			= "SELECT 仕分ＣＤ \n"
			+ " FROM ＣＭ１７仕分 \n"
			;

		private String[] Get_siwake(string[] sUser, OracleConnection conn2, string sHatuCd, string sTyakuCd)
		{
//			logWriter(sUser, INF, "仕分ＣＤ取得開始");

			string[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			string cmdQuery = GET_SIWAKE_SELECT
				+ " WHERE 発店所ＣＤ = '" + sHatuCd + "' \n"
				+ " AND 着店所ＣＤ = '" + sTyakuCd + "' \n"
				+ " AND 削除ＦＧ = '0' \n"
				;

			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力
			
			cmdQuery = GET_SIWAKE_SELECT
				+ " WHERE 発店所ＣＤ = :p_HatsutenCD \n"
				+ "   AND 着店所ＣＤ = :p_ChakutenCD \n"
				+ "   AND 削除ＦＧ = '0' \n"
				;
			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_HatsutenCD", OracleDbType.Char, sHatuCd,  ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_ChakutenCD", OracleDbType.Char, sTyakuCd, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			if(reader.Read())
			{
				sRet[0] = " ";
//				sRet[1] = reader.GetString(0).Trim();
				sRet[1] = reader.GetString(0);
			}
			else
			{
				sRet[0] = "仕分ＣＤを決められませんでした";
				sRet[1] = " ";
			}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

			return sRet;
		}
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 END

		/*********************************************************************
		 * 部門マスタ出荷日取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、出荷日
		 *
		 *********************************************************************/
		private String[] Get_bumonsyukka(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
		{
			string[] sRet = new string[2];

			string cmdQuery = "SELECT 出荷日 \n"
				+ " FROM ＣＭ０２部門 \n"
				+ " WHERE 会員ＣＤ   = '" + sKcode + "' \n"
				+ "   AND 部門ＣＤ   = '" + sBcode + "' \n"
				+ "   AND 削除ＦＧ   = '0' \n";

			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			OracleParameter[]	wk_opOraParam	= null;

			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery = "SELECT 出荷日 \n"
				+ " FROM ＣＭ０２部門 \n"
				+ " WHERE 会員ＣＤ   = :p_KaiinCD \n"
				+ "   AND 部門ＣＤ   = :p_BumonCD \n"
				+ "   AND 削除ＦＧ   = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			bool bRead = reader.Read();
			if(bRead == true)
			{
				sRet[0] = " ";
				sRet[1] = reader.GetString(0).Trim();
			}
			else
			{
				sRet[0] = "出荷日エラー";
				sRet[1] = "0";
			}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

			return sRet;
		}

		/*********************************************************************
		 * 出荷一覧取得（ＣＳＶ出力用）
		 * 引数：会員ＣＤ、部門ＣＤ、荷受人ＣＤ、荷送人ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日、状態
		 * 戻値：ステータス、登録日、ジャーナルＮＯ、荷受人ＣＤ...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_3
// ADD 2005.06.03 東都）小童谷 依頼主情報追加 START
//			= "SELECT J.登録日, TO_CHAR(\"ジャーナルＮＯ\"), J.荷受人ＣＤ, J.郵便番号, \n"
//			+       " '(' || TRIM(J.電話番号１) || ')' || TRIM(J.電話番号２) || '-' || J.電話番号３, \n"
//			+       " J.住所１, J.住所２, J.住所３, J.名前１, J.名前２, J.特殊計, J.着店ＣＤ, J.着店名, \n"
//			+       " J.荷送人ＣＤ, TO_CHAR(J.個数), TO_CHAR(J.重量), \n"
//			+       " J.指定日, J.輸送指示１, J.輸送指示２, J.品名記事１, J.品名記事２, J.品名記事３, \n"
//			+       " J.元着区分, TO_CHAR(J.保険金額), J.お客様出荷番号, \n"
//			+       " J.出荷日, J.得意先ＣＤ, J.部課ＣＤ, \n"
////			+       " CASE 送り状番号 WHEN ' ' THEN ' ' "
////			+       " ELSE SUBSTR(送り状番号,5,3) || '-' || SUBSTR(送り状番号,8,4) || '-' || SUBSTR(送り状番号,12,4) END \n"
//			+       " 送り状番号 \n"
//			+ " FROM \"ＳＴ０１出荷ジャーナル\" J \n";

			= "SELECT /*+ INDEX(J ST01IDX2) INDEX(N SM01PKEY) */ \n"
			+       " J.登録日, J.出荷日, 送り状番号, J.荷受人ＣＤ, J.郵便番号, \n"
			+       " '(' || TRIM(J.電話番号１) || ')' || TRIM(J.電話番号２) || '-' || J.電話番号３, \n"
			+       " J.住所１, J.住所２, J.住所３, J.名前１, J.名前２, J.特殊計, J.着店ＣＤ, J.着店名, \n"
			+       " J.荷送人ＣＤ, NVL(N.郵便番号, ' '), \n"
			+       " NVL(N.電話番号１,' '), NVL(N.電話番号２,' '), NVL(N.電話番号３,' '), \n"
			+       " NVL(N.住所１,' '), NVL(N.住所２,' '), NVL(N.名前１,' '), NVL(N.名前２,' '), \n"
			+       " TO_CHAR(J.個数), TO_CHAR(J.重量), \n"
			+       " J.指定日, J.輸送指示１, J.輸送指示２, J.品名記事１, J.品名記事２, J.品名記事３, \n"
			+       " J.元着区分, TO_CHAR(J.保険金額), J.お客様出荷番号, \n"
			+       " J.得意先ＣＤ, J.部課ＣＤ, J.才数 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
			+       " , J.運賃才数, J.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
			+       ", J.荷送人部署名, J.指定日区分, J.品名記事４, J.品名記事５, J.品名記事６ \n"
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）START
			+       ", J.発店ＣＤ, J.発店名 \n"
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）END
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 START
			+       ", DECODE(J.処理０３,'          ',' ',('20' || SUBSTR(J.処理０３,1,2) || '/' || SUBSTR(J.処理０３,3,2) || '/' || SUBSTR(J.処理０３,5,2) || ' ' || SUBSTR(J.処理０３,7,2) || ':' || SUBSTR(J.処理０３,9,2))) \n"
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" J,ＳＭ０１荷送人 N \n"
// ADD 2005.06.03 東都）小童谷 依頼主情報追加 START
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			;

		[WebMethod]
		public String[] Get_csvwrite(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
		{
			string[] sKey = new string[]{sKCode, sBCode, sTCode, sICode, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_csvwrite2(sUser, sKey);
		}

		[WebMethod]
		public String[] Get_csvwrite2(string[] sUser, string[] sKey)
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "ＣＳＶ出力２開始");
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string sTCode   = sKey[2];
			string sICode   = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s送り状番号開始 = ""; if(sKey.Length >  8) s送り状番号開始 = sKey[ 8];
			string s送り状番号終了 = ""; if(sKey.Length >  9) s送り状番号終了 = sKey[ 9];
			string sお客様番号開始 = ""; if(sKey.Length > 10) sお客様番号開始 = sKey[10];
			string sお客様番号終了 = ""; if(sKey.Length > 11) sお客様番号終了 = sKey[11];
			string s請求先ＣＤ     = ""; if(sKey.Length > 12) s請求先ＣＤ     = sKey[12];
			string s請求先部課ＣＤ = ""; if(sKey.Length > 13) s請求先部課ＣＤ = sKey[13];
			int    i帳票出力形式   = 0 ; if(sKey.Length > 14) i帳票出力形式   = int.Parse(sKey[14]);
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）START
			string sＣＳＶ出力形式 = ""; if(sKey.Length > 15) sＣＳＶ出力形式 = sKey[15];
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）END
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）START
			string s発店ＣＤ出力形式 = ""; if(sKey.Length > 16) s発店ＣＤ出力形式 = sKey[16];
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）END
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 START
			string s配完Ｓ出力形式 = ""; if(sKey.Length > 17) s配完Ｓ出力形式 = sKey[17];
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 END

			if(s送り状番号開始.Length == 0) s送り状番号開始 = s送り状番号終了;
			if(s送り状番号終了.Length == 0) s送り状番号終了 = s送り状番号開始;
			if(sお客様番号開始.Length == 0) sお客様番号開始 = sお客様番号終了;
			if(sお客様番号終了.Length == 0) sお客様番号終了 = sお客様番号開始;
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数重量 = 0;
			decimal d重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			decimal d才数 = 0;
//			string cmdQuery;
			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.部門ＣＤ = '" + sBCode + "' \n");
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
				if(s送り状番号開始.Length > 0){
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 START
					if(s送り状番号開始.Length >= 11){
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 END
						if(s送り状番号開始 == s送り状番号終了){
							sbQuery.Append(" AND J.送り状番号 = '0000"+ s送り状番号開始 + "' \n");
						}else{
							sbQuery.Append(" AND J.送り状番号 BETWEEN '0000"+ s送り状番号開始
														 + "' AND '0000"+ s送り状番号終了 + "' \n");
						}
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 START
					}else if(s送り状番号開始.Length >= 6){
						sbQuery.Append(" AND J.送り状番号 BETWEEN '0000"+ s送り状番号開始.PadRight(11,'0')
													 + "' AND '0000"+ s送り状番号終了.PadRight(11,'9') + "' \n");
					}else if(s送り状番号開始.Length == 4
							|| s送り状番号開始.Length == 5){
						if(s送り状番号開始 == s送り状番号終了){
							sbQuery.Append(" AND SUBSTR(J.送り状番号,"
							 +(5+11-s送り状番号開始.Length)+","+s送り状番号開始.Length
							 + ") = '" + s送り状番号開始 + "' \n");
						}else{
							sbQuery.Append(" AND SUBSTR(J.送り状番号,"
							 +(5+11-s送り状番号開始.Length)+","+s送り状番号開始.Length
							 + ") BETWEEN '" + s送り状番号開始
							 + "' AND '"+ s送り状番号終了 + "' \n");
						}
					}else{
					}
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 END
				}
				if(sお客様番号開始.Length > 0){
					if(sお客様番号開始 == sお客様番号終了){
						sbQuery.Append(" AND J.お客様出荷番号 = '"+ sお客様番号開始 + "' \n");
					}else{
						sbQuery.Append(" AND J.お客様出荷番号 BETWEEN '"+ sお客様番号開始
													 + "' AND '"+ sお客様番号終了 + "' \n");
					}
				}
				if(s請求先ＣＤ.Length > 0){
					sbQuery.Append(" AND J.得意先ＣＤ = '"+ s請求先ＣＤ + "' \n");
				}
				if(s請求先部課ＣＤ.Length > 0){
					sbQuery.Append(" AND J.部課ＣＤ = '"+ s請求先部課ＣＤ + "' \n");
				}
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND J.荷受人ＣＤ = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND J.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND J.荷受人ＣＤ = '"+ sTCode + "' \n");
					sbQuery.Append(" AND J.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(iSyuka == 0)
					sbQuery.Append(" AND J.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sJyoutai != "00")
				{
// ADD 2008.07.09 東都）高木 未発行分を除外する START
					if(sJyoutai == "aa")
						sbQuery.Append(" AND J.状態 <> '01' \n");
					else
// ADD 2008.07.09 東都）高木 未発行分を除外する END
						sbQuery.Append(" AND J.状態 = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND J.削除ＦＧ = '0' \n");
// ADD 2005.06.03 東都）小童谷 依頼主情報取得 START
				sbQuery.Append(" AND J.荷送人ＣＤ     = N.荷送人ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.会員ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.部門ＣＤ(+) \n");
				sbQuery.Append(" AND '0' = N.削除ＦＧ(+) ");
// ADD 2005.06.03 東都）小童谷 依頼主情報取得 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(" AND J.会員ＣＤ     = CM01.会員ＣＤ(+) \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

				OracleDataReader reader;
// MOD 2009.11.04 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
//				if(iSyuka == 0)
//				{
//					sbQuery2.Append(GET_SYUKKA_SELECT_3);
//					sbQuery2.Append(sbQuery);
//					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
//					reader = CmdSelect(sUser, conn2, sbQuery2);
//				}
//				else
//				{
//					sbQuery2.Append(GET_SYUKKA_SELECT_3);
//					sbQuery2.Append(sbQuery);
//					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
//					reader = CmdSelect(sUser, conn2, sbQuery2);
//				}
				sbQuery2.Append(GET_SYUKKA_SELECT_3);
				sbQuery2.Append(sbQuery);
				switch(i帳票出力形式){
				case 1:		//ご依頼主別
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY 出荷日,荷送人ＣＤ,登録日,\"ジャーナルＮＯ\" ");
					}else{
						sbQuery2.Append(" ORDER BY 登録日,荷送人ＣＤ,\"ジャーナルＮＯ\" ");
					}
					break;
				case 2:		//請求先別
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY 出荷日,得意先ＣＤ,部課ＣＤ,登録日,\"ジャーナルＮＯ\" ");
					}else{
						sbQuery2.Append(" ORDER BY 登録日,得意先ＣＤ,部課ＣＤ,\"ジャーナルＮＯ\" ");
					}
					break;
				case 3:		//お届け先別
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY 出荷日,荷受人ＣＤ,登録日,\"ジャーナルＮＯ\" ");
					}else{
						sbQuery2.Append(" ORDER BY 登録日,荷受人ＣＤ,\"ジャーナルＮＯ\" ");
					}
					break;
				default:	//指定なし
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
					}
					break;
				}
				reader = CmdSelect(sUser, conn2, sbQuery2);
// MOD 2009.11.04 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END

				StringBuilder sbData = new StringBuilder(1024);
				while (reader.Read())
				{
/*
					sList.Add(sDbl + reader.GetString(0).Trim() + sDbl              //登録日
						+ sKanma + sDbl + sSng + reader.GetString(1).Trim() + sDbl  //ジャーナルＮＯ
						+ sKanma + sDbl + sSng + reader.GetString(2).Trim() + sDbl  //荷受人ＣＤ
						+ sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl  //郵便番号
						+ sKanma + sDbl + reader.GetString(4).Trim() + sDbl         //電話番号
						+ sKanma + sDbl + reader.GetString(5).Trim() + sDbl         //住所１
						+ sKanma + sDbl + reader.GetString(6).Trim() + sDbl         //住所２
						+ sKanma + sDbl + reader.GetString(7).Trim() + sDbl         //住所３
						+ sKanma + sDbl + reader.GetString(8).Trim() + sDbl         //名前１
						+ sKanma + sDbl + reader.GetString(9).Trim() + sDbl         //名前２
						+ sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl //特殊計
						+ sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl //着店ＣＤ
						+ sKanma + sDbl + reader.GetString(12).Trim() + sDbl        //着店名
						+ sKanma + sDbl + sSng + reader.GetString(13).Trim() + sDbl //荷送人ＣＤ
						+ sKanma + reader.GetString(14)         //個数
						+ sKanma + reader.GetString(15)         //重量
						+ sKanma + sDbl + reader.GetString(16).Trim() + sDbl        //指定日
						+ sKanma + sDbl + sSng + reader.GetString(17).Trim() + sDbl //輸送指示１
						+ sKanma + sDbl + sSng + reader.GetString(18).Trim() + sDbl //輸送指示２
						+ sKanma + sDbl + sSng + reader.GetString(19).Trim() + sDbl //品名記事１
						+ sKanma + sDbl + sSng + reader.GetString(20).Trim() + sDbl //品名記事２
						+ sKanma + sDbl + sSng + reader.GetString(21).Trim() + sDbl //品名記事３
						+ sKanma + sDbl + reader.GetString(22).Trim() + sDbl       //元着区分
						+ sKanma + reader.GetString(23)         //保険金額
						+ sKanma + sDbl + sSng + reader.GetString(24).Trim() + sDbl //お客様出荷番号
						+ sKanma + sDbl + reader.GetString(25).Trim() + sDbl        //出荷日
						+ sKanma + sDbl + sSng + reader.GetString(26).Trim() + sDbl //得意先ＣＤ
						+ sKanma + sDbl + sSng + reader.GetString(27).Trim() + sDbl //部課ＣＤ
						+ sKanma + sDbl + reader.GetString(28).Trim() + sDbl        //送り状番号
						);
*/
					sbData = new StringBuilder(1024);
// MOD 2005.06.03 東都）小童谷 依頼主情報追加 START
/*					sbData.Append(sDbl + sSng + reader.GetString(0).Trim() + sDbl);				// 登録日
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(1).Trim() + sDbl);	// ジャーナルＮＯ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(2).Trim() + sDbl);	// 荷受人ＣＤ
					sbData.Append(sKanma + sDbl + reader.GetString(3).Trim() + sDbl);			// 郵便番号
					sbData.Append(sKanma + sDbl + reader.GetString(4).Trim() + sDbl);			// 電話番号
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// 住所１
					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// 住所２
					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// 住所３
					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// 名前１
					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// 名前２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl);	// 特殊計
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// 着店ＣＤ
					sbData.Append(sKanma + sDbl + reader.GetString(12).Trim() + sDbl       );	// 着店名
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(13).Trim() + sDbl);	// 荷送人ＣＤ
					sbData.Append(sKanma + reader.GetString(14)                            );	// 個数
					sbData.Append(sKanma + reader.GetString(15)                            );	// 重量
					sbData.Append(sKanma + sDbl + reader.GetString(16).Trim() + sDbl       );	// 指定日
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(17).TrimEnd() + sDbl);	// 輸送指示１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(18).TrimEnd() + sDbl);	// 輸送指示２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(19).TrimEnd() + sDbl);	// 品名記事１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(20).TrimEnd() + sDbl);	// 品名記事２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(21).TrimEnd() + sDbl);	// 品名記事３
					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl       );	// 元着区分
					sbData.Append(sKanma + reader.GetString(23)                            );	// 保険金額
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(24).Trim() + sDbl);	// お客様出荷番号
					sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// 出荷日
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).Trim() + sDbl);	// 得意先ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).Trim() + sDbl);	// 部課ＣＤ
					string sNo = reader.GetString(28).Trim();									// 送り状番号(XXX-XXXX-XXXX)
					if(sNo.Length == 15)
					{
						sbData.Append(sKanma + sDbl + sNo.Substring(4,3)
							+ "-" + sNo.Substring(7,4) + "-" + sNo.Substring(11) + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
*/
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）START
// MOD 2011.07.14 東都）高木 記事行の追加 START
//				if(sＣＳＶ出力形式.Equals("1")){
				if(sＣＳＶ出力形式.Equals("1") || sＣＳＶ出力形式.Equals("3")){
// MOD 2011.07.14 東都）高木 記事行の追加 END
					sbData.Append(sDbl + sSng + reader.GetString(3).Trim() + sDbl);		// 荷受人ＣＤ
//					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);	// 荷受人電話番号
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);	// 荷受人住所１
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);	// 荷受人住所２
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);	// 荷受人住所３
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);	// 荷受人名前１
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);	// 荷受人名前２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(5).Trim() + sDbl);	// 荷受人電話番号
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(6).Trim() + sDbl);	// 荷受人住所１
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(7).Trim() + sDbl);	// 荷受人住所２
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(8).Trim() + sDbl);	// 荷受人住所３
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(9).Trim() + sDbl);	// 荷受人名前１
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl);	// 荷受人名前２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(6).TrimEnd() + sDbl);  // 荷受人住所１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(7).TrimEnd() + sDbl);  // 荷受人住所２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(8).TrimEnd() + sDbl);  // 荷受人住所３
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(9).TrimEnd() + sDbl);  // 荷受人名前１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).TrimEnd() + sDbl); // 荷受人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// 荷受人郵便番号
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// 特殊計
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// 着店ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// 荷送人ＣＤ
// MOD 2011.07.14 東都）高木 記事行の追加 START
				if(sＣＳＶ出力形式.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(40).TrimEnd() + sDbl); // 荷送担当者
				}
// MOD 2011.07.14 東都）高木 記事行の追加 END
					sbData.Append(sKanma + reader.GetString(23).Trim()                     );	// 個数
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					sbData.Append(sKanma + reader.GetDecimal(36).ToString().Trim()         );	// 才数
//					sbData.Append(sKanma + reader.GetString(24).Trim()                     );	// 重量
					s運賃才数 = reader.GetString(37).TrimEnd();
					s運賃重量 = reader.GetString(38).TrimEnd();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(39).TrimEnd();
					if(s重量入力制御 == "1"
					&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						sbData.Append(sKanma + reader.GetDecimal(36).ToString().TrimEnd());	// 才数
						sbData.Append(sKanma + reader.GetString(24).TrimEnd());				// 重量
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						d才数 = 0;
						d重量 = 0;
						if(s運賃才数.Length > 0){
							try{
								d才数 = Decimal.Parse(s運賃才数);
							}catch(Exception){}
						}
						if(s運賃重量.Length > 0){
							try{
								d重量 = Decimal.Parse(s運賃重量);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d才数.ToString());		// 才数
						sbData.Append(sKanma + d重量.ToString());		// 重量
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// 輸送指示１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// 輸送指示２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// 品名記事１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// 品名記事２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// 品名記事３
// MOD 2011.07.14 東都）高木 記事行の追加 START
				if(sＣＳＶ出力形式.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(42).TrimEnd() + sDbl);	// 品名記事４
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(43).TrimEnd() + sDbl);	// 品名記事５
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(44).TrimEnd() + sDbl);	// 品名記事６
				}
// MOD 2011.07.14 東都）高木 記事行の追加 END

					if(reader.GetString(25).Trim() == "0"){
						sbData.Append(sKanma + sDbl + sDbl);										// 指定日
					}else{
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// 指定日
					}
// MOD 2011.07.14 東都）高木 記事行の追加 START
				if(sＣＳＶ出力形式.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(41).TrimEnd() + sDbl); // 必着区分
				}
// MOD 2011.07.14 東都）高木 記事行の追加 END

					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// お客様出荷番号
// MOD 2011.07.14 東都）高木 記事行の追加 START
//					sbData.Append(sKanma + sDbl + sDbl);										// 予備
				if(sＣＳＶ出力形式.Equals("1")){
					sbData.Append(sKanma + sDbl + sDbl);										// 予備
				}
// MOD 2011.07.14 東都）高木 記事行の追加 END
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// 元着区分
					sbData.Append(sKanma + reader.GetString(32).Trim()                     );	// 保険金額
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl);	// 出荷日
					sbData.Append(sKanma + sDbl + sDbl);								// 登録日（省略）
				}else{
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）END
// MOD 2005.07.21 東都）高木 フォーマット確認 START
//					sbData.Append(sDbl + sSng + reader.GetString(0).Trim() + sDbl);				// 登録日
					sbData.Append(sDbl + reader.GetString(0).Trim() + sDbl);					// 登録日
// MOD 2005.07.21 東都）高木 フォーマット確認 END
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl       );	// 出荷日
					string sNo = reader.GetString(2).Trim();									// 送り状番号(XXX-XXXX-XXXX)
					if(sNo.Length == 15)
					{
						sbData.Append(sKanma + sDbl + sNo.Substring(4,3)
							+ "-" + sNo.Substring(7,4) + "-" + sNo.Substring(11) + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl);	// 荷受人ＣＤ
// MOD 2005.07.21 東都）高木 フォーマット確認 START
//					sbData.Append(sKanma + sDbl + reader.GetString(4).Trim() + sDbl);			// 荷受人郵便番号
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// 荷受人郵便番号
// MOD 2005.07.21 東都）高木 フォーマット確認 END
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// 荷受人電話番号
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// 荷受人住所１
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// 荷受人住所２
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// 荷受人住所３
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// 荷受人名前１
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);			// 荷受人名前２
					sbData.Append(sKanma + sDbl + reader.GetString(6).TrimEnd() + sDbl);  // 荷受人住所１
					sbData.Append(sKanma + sDbl + reader.GetString(7).TrimEnd() + sDbl);  // 荷受人住所２
					sbData.Append(sKanma + sDbl + reader.GetString(8).TrimEnd() + sDbl);  // 荷受人住所３
					sbData.Append(sKanma + sDbl + reader.GetString(9).TrimEnd() + sDbl);  // 荷受人名前１
					sbData.Append(sKanma + sDbl + reader.GetString(10).TrimEnd() + sDbl); // 荷受人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// 特殊計
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// 着店ＣＤ
					sbData.Append(sKanma + sDbl + reader.GetString(13).Trim() + sDbl       );	// 着店名
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// 荷送人ＣＤ
// MOD 2005.07.21 東都）高木 フォーマット確認 START
//					sbData.Append(sKanma + sDbl + reader.GetString(15).Trim() + sDbl);			// 荷送人郵便番号
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(15).Trim() + sDbl);	// 荷送人郵便番号
// MOD 2005.07.21 東都）高木 フォーマット確認 END

					string sTel = reader.GetString(16).Trim();									// 荷送人電話番号
					if(sTel.Length != 0)
					{
						sbData.Append(sKanma + sDbl + "(" + sTel + ")"
							+ "-" + reader.GetString(17).Trim() + "-" + reader.GetString(18).Trim() + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}

// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sbData.Append(sKanma + sDbl + reader.GetString(19).Trim() + sDbl);			// 荷送人住所１
//					sbData.Append(sKanma + sDbl + reader.GetString(20).Trim() + sDbl);			// 荷送人住所２
//					sbData.Append(sKanma + sDbl + reader.GetString(21).Trim() + sDbl);			// 荷送人名前１
//					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl);			// 荷送人名前２
					sbData.Append(sKanma + sDbl + reader.GetString(19).TrimEnd() + sDbl); // 荷送人住所１
					sbData.Append(sKanma + sDbl + reader.GetString(20).TrimEnd() + sDbl); // 荷送人住所２
					sbData.Append(sKanma + sDbl + reader.GetString(21).TrimEnd() + sDbl); // 荷送人名前１
					sbData.Append(sKanma + sDbl + reader.GetString(22).TrimEnd() + sDbl); // 荷送人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sbData.Append(sKanma + reader.GetString(23)                            );	// 個数
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					d才数 = reader.GetDecimal(36);													// 才数
//					d才数 = d才数 * 8;
//					if(d才数 == 0)
//						sbData.Append(sKanma + reader.GetString(24)                            );	// 重量
//					else
//						sbData.Append(sKanma + d才数.ToString()                            );
					s運賃才数 = reader.GetString(37).TrimEnd();
					s運賃重量 = reader.GetString(38).TrimEnd();
					d才数重量 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(39).TrimEnd();
					if(s重量入力制御 == "1"
					&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						d才数重量 += (reader.GetDecimal(36) * 8);		// 才数
						if(reader.GetString(24).TrimEnd().Length > 0){	// 重量
							try{
								d才数重量 += Decimal.Parse(reader.GetString(24).TrimEnd());
							}catch(Exception){}
						}
						sbData.Append(sKanma + d才数重量.ToString());	// 重量
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						if(s運賃才数.Length > 0){
							try{
								d才数重量 += (Decimal.Parse(s運賃才数) * 8);
							}catch(Exception){}
						}
						if(s運賃重量.Length > 0){
							try{
								d才数重量 += Decimal.Parse(s運賃重量);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d才数重量.ToString());		// 重量
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2005.07.21 東都）高木 フォーマット確認 START
//					sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// 指定日
					if(reader.GetString(25).Trim() == "0")
						sbData.Append(sKanma + sDbl + sDbl);										// 指定日
					else
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// 指定日
// MOD 2005.07.21 東都）高木 フォーマット確認 END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// 輸送指示１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// 輸送指示２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// 品名記事１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// 品名記事２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// 品名記事３
// MOD 2011.07.14 東都）高木 記事行の追加 START
				if(sＣＳＶ出力形式.Equals("2")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(42).TrimEnd() + sDbl);	// 品名記事４
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(43).TrimEnd() + sDbl);	// 品名記事５
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(44).TrimEnd() + sDbl);	// 品名記事６
				}
// MOD 2011.07.14 東都）高木 記事行の追加 END
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// 元着区分
					sbData.Append(sKanma + reader.GetString(32)                            );	// 保険金額
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// お客様出荷番号
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(34).Trim() + sDbl);	// 得意先ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(35).Trim() + sDbl);	// 部課ＣＤ
// MOD 2005.06.03 東都）小童谷 依頼主情報追加 END
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）START
				}
// MOD 2010.02.01 東都）高木 オプションの項目追加（ＣＳＶ出力形式）END
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）START
				if(s発店ＣＤ出力形式.Equals("1"))
				{
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(45).TrimEnd() + sDbl);	// 発店ＣＤ
					sbData.Append(sKanma + sDbl + reader.GetString(46).TrimEnd() + sDbl);	// 発店名
				}
// MOD 2013.04.04 TDI）綱澤 出力レイアウト追加（グローバル専用）END
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 START
				if(s配完Ｓ出力形式.Equals("1"))
				{
					sbData.Append(sKanma + sDbl + reader.GetString(47).Trim() + sDbl       );	// 配完日付・時刻
				}
// MOD 2013.10.07 BEVAS）高杉 ＣＳＶ出力に配完日付・時刻を追加 END
					sList.Add(sbData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * アップロードデータ追加
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_autoEntryData(string[] sUser, string[] sList)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "自動出荷登録データ追加開始");

			OracleConnection conn2 = null;
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 START
//			string[] sRet = new string[1];
//			string s更新日時 = System.DateTime.Now.ToString("yyyyMMddHHmmss");
			string[] sRet = new string[1 + sList.Length * 2];
			int iRetCnt = 1;
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

//保留 ADD 2007.04.27 東都）高木 ORA-01000 対応 START
//			string s発店ＣＤ旧 = "";
//			string s着店ＣＤ旧 = "";
//			string s仕分ＣＤ旧 = " ";
//保留 ADD 2007.04.27 東都）高木 ORA-01000 対応 END
			sRet[0] = "";
			try
			{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				string s重量入力制御 = " ";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				for (int i = 0; i < sList.Length; i++)
				{
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 START
					if(sList[i] == null) break;
					if(sList[i].Length == 0) break;
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 END
// DEL 2005.06.08 東都）高木 未使用の為削除 START
//					string s特殊計 = " ";
// DEL 2005.06.08 東都）高木 未使用の為削除 END
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 START
					string s登録日;
					int i管理ＮＯ;
					string s日付;
					string[] sData = sList[i].Split(',');
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
///					string s重量入力制御 = (sData.Length > 46) ? sData[46] : "0";
///					string s重量入力制御 = (sData.Length > 46) ? sData[46] : " ";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
					string cmdQuery = "";
					OracleDataReader reader;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					if(s重量入力制御 == " "){
						cmdQuery = "SELECT CM01.保留印刷ＦＧ \n"
							+ "  FROM ＣＭ０１会員 CM01 \n"
							+ " WHERE CM01.会員ＣＤ = '" + sData[0]  +"' \n"
							;
						reader = CmdSelect(sUser, conn2, cmdQuery);
						if(reader.Read()){
							s重量入力制御 = reader.GetString(0);
						}
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

					//ジャーナルＮＯ取得
					cmdQuery
						= "SELECT \"ジャーナルＮＯ登録日\",\"ジャーナルＮＯ管理\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM ＣＭ０２部門 \n"
						+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
						+ "   AND 部門ＣＤ = '" + sData[1] +"' \n"
						+ "   AND 削除ＦＧ = '0'"
						+ "   FOR UPDATE ";

					reader = CmdSelect(sUser, conn2, cmdQuery);
					reader.Read();
					s登録日   = reader.GetString(0).Trim();
					i管理ＮＯ = reader.GetInt32(1);
					s日付     = reader.GetString(2);
// ADD 2005.06.08 東都）伊賀 ORA-01000対策 START
					reader.Close();
// ADD 2005.06.08 東都）伊賀 ORA-01000対策 END
					if(s登録日 == s日付)
						i管理ＮＯ++;
					else
					{
						s登録日 = s日付;
						i管理ＮＯ = 1;
					}

					cmdQuery 
						= "UPDATE ＣＭ０２部門 \n"
						+    "SET \"ジャーナルＮＯ登録日\"  = '" + s登録日 +"', \n"
						+        "\"ジャーナルＮＯ管理\"    = " + i管理ＮＯ +", \n"
// MOD 2010.11.10 東都）高木 更新者、更新ＰＧの項目の修正 START
//						+        "更新ＰＧ                  = '" + sData[40] +"', \n"
//						+        "更新者                    = '" + sData[41] +"', \n"
						+        "更新ＰＧ                  = '" + sData[44] +"', \n"
						+        "更新者                    = '" + sData[45] +"', \n"
// MOD 2010.11.10 東都）高木 更新者、更新ＰＧの項目の修正 END
						+        "更新日時                  =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE 会員ＣＤ       = '" + sData[0] +"' \n"
						+ "   AND 部門ＣＤ       = '" + sData[1] +"' \n"
						+ "   AND 削除ＦＧ = '0'";

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
//					string s登録日 = "";
//					int i管理ＮＯ  = 0;
//					string s日付   = "";
//					string[] sData = sList[i].Split(',');
//					string cmdQuery = "";
//					string[] sRet2 = Get_JurnalNo(sUser, sData[0], sData[1], sData[40]);
//					if(sRet2[0].Length == 4){
//						s登録日   = sRet2[1];
//						s日付     = sRet2[1];
//						i管理ＮＯ = int.Parse(sRet2[2]);
//					}else{
//						tran.Rollback();
//						return sRet2;
//					}
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 END
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 START
					//仕分ＣＤ取得
					string s発店ＣＤ = sData[21];
					string s着店ＣＤ = sData[15];
					string s仕分ＣＤ = " ";
					if(s発店ＣＤ.Trim().Length > 0 && s着店ＣＤ.Trim().Length > 0){
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 START
						string[] sRetSiwake = Get_siwake(sUser, conn2, s発店ＣＤ, s着店ＣＤ);
//						string[] sRetSiwake = new string[2]{""," "};
//						if(s発店ＣＤ旧.Length > 0 && s着店ＣＤ旧.Length > 0
//							&& s発店ＣＤ == s発店ＣＤ旧 && s着店ＣＤ == s着店ＣＤ旧)
//						{
//							sRetSiwake[1] = s仕分ＣＤ旧;
//						}
//						else
//						{
//							sRetSiwake = Get_siwake(sUser, conn2, s発店ＣＤ, s着店ＣＤ);
//							s発店ＣＤ旧 = s発店ＣＤ;
//							s着店ＣＤ旧 = s着店ＣＤ;
//							s仕分ＣＤ旧 = sRetSiwake[1];
//						}
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 END
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 東都）高木 仕分ＣＤの追加（エラー表示障害対応） END
//						if(sRet[0] != " ") return sRet;
						s仕分ＣＤ = sRetSiwake[1];
					}
// ADD 2007.02.08 東都）高木 仕分ＣＤの追加 END
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 START
					if(sData[4] != " ")
					{
						cmdQuery
							= "UPDATE ＳＭ０２荷受人 \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 START
//							+ " SET 名前３ = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET 登録ＰＧ = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 東都）高木 荷受人マスタの[登録ＰＧ]に最終使用日を更新 END
							+ " WHERE 会員ＣＤ = '" + sData[0] +"' \n"
							+ " AND 部門ＣＤ   = '" + sData[1] +"' \n"
							+ " AND 荷受人ＣＤ = '" + sData[4] +"' \n"
							+ " AND 削除ＦＧ   = '0'";
						try{
							int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
						}catch(Exception){
							;
						}
					}
// ADD 2009.01.30 東都）高木 [名前３]に最終利用年月を更新 END

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
// MOD 2011.07.14 東都）高木 記事行の追加 START
//					// 処理０２に才数および重量の参考値を入れる
//					string s才数 = "";
//					string s重量 = "";
//					string s才数重量 = "";
//					try{
//						s才数 = sData[27].Trim().PadLeft(5,'0');
//						s重量 = sData[28].Trim().PadLeft(5,'0');
//						s才数重量 = s才数.Substring(0,5)
//									+ s重量.Substring(0,5);
//					}catch(Exception){
//					}
// MOD 2011.07.14 東都）高木 記事行の追加 END
//注意：リカバリーができなくなる
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//					sData[27] = "0"; // 才数
//					sData[28] = "0"; // 重量
					if(s重量入力制御 == "0"){
						sData[27] = "0"; // 才数
						sData[28] = "0"; // 重量
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					string s品名記事４ = (sData.Length > 47) ? sData[47] : " ";
					string s品名記事５ = (sData.Length > 48) ? sData[48] : " ";
					string s品名記事６ = (sData.Length > 49) ? sData[49] : " ";
					if(s品名記事４.Length == 0) s品名記事４ = " ";
// MOD 2011.07.14 東都）高木 記事行の追加 END
					cmdQuery 
						= "INSERT INTO \"ＳＴ０１出荷ジャーナル\" \n"
// MOD 2010.10.13 東都）高木 [品名記事４]など項目追加 START
						+ "(会員ＣＤ, 部門ＣＤ, 登録日, \"ジャーナルＮＯ\", 出荷日 \n"
						+ ", お客様出荷番号, 荷受人ＣＤ \n"
						+ ", 電話番号１, 電話番号２, 電話番号３, ＦＡＸ番号１, ＦＡＸ番号２, ＦＡＸ番号３ \n"
						+ ", 住所ＣＤ, 住所１, 住所２, 住所３ \n"
						+ ", 名前１, 名前２, 名前３, 郵便番号 \n"
						+ ", 着店ＣＤ, 着店名, 特殊計 \n"
						+ ", 荷送人ＣＤ, 荷送人部署名 \n"
						+ ", 集約店ＣＤ, 発店ＣＤ, 発店名 \n"
						+ ", 得意先ＣＤ, 部課ＣＤ, 部課名 \n"
						+ ", 個数, 才数, 重量, ユニット \n"
						+ ", 指定日, 指定日区分 \n"
						+ ", 輸送指示ＣＤ１, 輸送指示１ \n"
						+ ", 輸送指示ＣＤ２, 輸送指示２ \n"
						+ ", 品名記事１, 品名記事２, 品名記事３ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 START
						+ ", 品名記事４, 品名記事５, 品名記事６ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
						+ ", 元着区分, 保険金額, 運賃, 中継, 諸料金 \n"
						+ ", 仕分ＣＤ, 送り状番号, 送り状区分 \n"
						+ ", 送り状発行済ＦＧ, 出荷済ＦＧ, 送信済ＦＧ, 一括出荷ＦＧ \n"
						+ ", 状態, 詳細状態 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
						+ ", 処理０２ \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷登録時は、処理０４に「0」を設定する
						+ ",処理０４ \n"
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
						+ ", 削除ＦＧ, 登録日時, 登録ＰＧ, 登録者 \n"
						+ ", 更新日時, 更新ＰＧ, 更新者 \n"
						+ ") \n"
// MOD 2010.10.13 東都）高木 [品名記事４]など項目追加 END
						+ "VALUES ('" + sData[0]  +"','" + sData[1]  +"','" + s日付 +"'," + i管理ＮＯ +",'" + sData[2] +"', \n"
						+         "'" + sData[3]  +"','" + sData[4]  +"', \n"
						+         "'" + sData[5]  +"','" + sData[6]  +"','" + sData[7] +"',' ',' ',' ', \n"
						+         "'" + sData[8]  +"','" + sData[9]  +"','" + sData[10] +"','" + sData[11] +"', \n"
						+         "'" + sData[12] +"','" + sData[13] +"',' ', '" + sData[14] +"', \n"
						+         "'" + sData[15] +"','" + sData[16] +"','" + sData[17] +"', \n"
						+         "'" + sData[18] +"','" + sData[19] +"', \n"
						+         "'" + sData[20] +"','" + sData[21] +"','" + sData[22] +"', \n"
						+         "'" + sData[23] +"','" + sData[24] +"','" + sData[25] +"', \n"
						+         ""  + sData[26] +","   + sData[27] +","   + sData[28] +",0, \n"
// MOD 2005.06.20 東都）伊賀 レイアウト変更 START
// MOD 2005.05.31 東都）伊賀 指定日区分追加 START
//						+         "'" + sData[29] +"',' ','" + sData[30] +"',' ','" + sData[31] +"', \n"
//						+         "'" + sData[29] +"','0','000','" + sData[30] +"','000','" + sData[31] +"', \n"
						+         "'" + sData[29] +"','" + sData[30] +"','" + sData[31] +"','" + sData[32] +"','" + sData[33] +"','" + sData[34] +"', \n"
// MOD 2005.05.31 東都）伊賀 指定日区分追加 END
// MOD 2005.06.20 東都）伊賀 レイアウト変更 END
						+         "'" + sData[35] +"','" + sData[36] +"','" + sData[37] +"', \n"
// MOD 2011.07.14 東都）高木 記事行の追加 START
						+         "'" + s品名記事４ +"','"+ s品名記事５ +"','"+ s品名記事６ +"', \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
						+         "'" + sData[38] +"',"  + sData[39] +",0,0,0, \n"						//運賃　中継　諸料金
// MOD 2007.02.08 東都）高木 仕分ＣＤの追加 START
//						+         "' ',' ',' ',"														//仕分ＣＤ  送り状番号  送り状区分
						+         "'" + s仕分ＣＤ + "',' ',' ',"  //  仕分ＣＤ  送り状番号  送り状区分
// MOD 2007.02.08 東都）高木 仕分ＣＤの追加 END
						+         "'" + sData[41] +"','" + sData[42] +"', '0', '" + sData[43] +"', \n"  //送信済ＦＧ
						+         "'01','  ', \n"														//状態　詳細状態
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
// MOD 2011.07.14 東都）高木 記事行の追加 START
//						+         "'" + s才数重量 + "', \n" // 処理０２
						+         "' ', \n" // 処理０２
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 START
						//出荷登録時は、処理０４に「0」を設定する
						+         "'0', \n" // 処理０４
// MOD 2016.06.24 bevas) 松本 出荷修正データを支店へ正常に反映できるように対応 END
						+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[44] +"','" + sData[45] +"', \n"
						+             "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[44] +"','" + sData[45] +"')";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "自動出荷登録["+sData[1]+"]["+s日付+"]["+i管理ＮＯ+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 START
					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//保留 MOD 2007.04.27 東都）高木 ORA-01000 対応 END
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 START
					// 管理番号を保持する
					sRet[iRetCnt++] = s日付.Trim();
					sRet[iRetCnt++] = i管理ＮＯ.ToString().Trim();
// MOD 2010.04.07 東都）高木 出荷ＣＳＶ自動印刷 END
				}
				tran.Commit();
				logWriter(sUser, INF, "正常終了");
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}
// ADD 2005.06.02 東都）伊賀 内容確認処理見直し START
		/*********************************************************************
		 * 自動出荷登録用住所取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、出荷日
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_autoEntryPref(string[] sUser, string sYcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "住所取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			string cmdQuery = "";
			try
			{
				cmdQuery = "SELECT CM14.都道府県ＣＤ,CM14.市区町村ＣＤ,CM14.大字通称ＣＤ \n"
					+      ",NVL(CM10.店所ＣＤ,' '),NVL(CM10.店所名,' ') \n"
					+      " FROM ＣＭ１４郵便番号 CM14 \n"
					+      " LEFT JOIN ＣＭ１０店所 CM10 \n"
					+        "  ON CM14.店所ＣＤ = CM10.店所ＣＤ \n"
					+        " AND '0' = CM10.削除ＦＧ \n"
					+      " WHERE CM14.郵便番号 = '" + sYcode + "' \n"
					+        " AND CM14.削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim()	// 都道府県ＣＤ
							+ reader.GetString(1).Trim()	// 市区町村ＣＤ
							+ reader.GetString(2).Trim();	// 大字通称ＣＤ
					sRet[2] = reader.GetString(3).Trim();	// 店所ＣＤ
					sRet[3] = reader.GetString(4).Trim();	// 店所名

					sRet[0] = "正常終了";
				}else{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * 自動出荷登録用依頼主取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、出荷日
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_autoEntryClaim(string[] sUser, string sKcode, string sBcode, string sIcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "依頼主情報取得開始");

			OracleConnection conn2 = null;
// MOD 2016.02.02 BEVAS）松本 荷送人マスタの固定重量、固定才数の考慮追加 START
//			string[] sRet = new string[4];
			string[] sRet = new string[6];
// MOD 2016.02.02 BEVAS）松本 荷送人マスタの固定重量、固定才数の考慮追加 END

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			try
			{
// MOD 2016.02.02 BEVAS）松本 荷送人マスタ取得項目追加（重量、才数） START
//// MOD 2010.07.30 東都）高木 自動出荷時の荷送人情報取得の訂正 START
////				string cmdQuery = "SELECT SM01.得意先ＣＤ,SM01.得意先部課ＣＤ,SM04.得意先部課名 \n"
//				string cmdQuery = "SELECT SM01.得意先ＣＤ, SM01.得意先部課ＣＤ, NVL(SM04.得意先部課名,' ') \n"
				string cmdQuery = "SELECT SM01.得意先ＣＤ, SM01.得意先部課ＣＤ, NVL(SM04.得意先部課名,' ') , SM01.重量, SM01.才数 \n"
//// MOD 2010.07.30 東都）高木 自動出荷時の荷送人情報取得の訂正 END
// MOD 2016.02.02 BEVAS）松本 荷送人マスタ取得項目追加（重量、才数） END
					+ " FROM ＳＭ０１荷送人 SM01 \n"
					+ " LEFT JOIN ＣＭ０２部門 CM02 \n"
					+   " ON SM01.会員ＣＤ = CM02.会員ＣＤ \n"
					+  " AND SM01.部門ＣＤ = CM02.部門ＣＤ \n"
					+  " AND '0' = CM02.削除ＦＧ \n"
					+ " LEFT JOIN ＳＭ０４請求先 SM04 \n"
					+   " ON CM02.会員ＣＤ = SM04.会員ＣＤ \n"
					+  " AND CM02.郵便番号 = SM04.郵便番号 \n"
					+  " AND SM01.得意先ＣＤ = SM04.得意先ＣＤ \n"
					+  " AND SM01.得意先部課ＣＤ = SM04.得意先部課ＣＤ \n"
					+  " AND '0' = SM04.削除ＦＧ \n"
					+ " WHERE SM01.荷送人ＣＤ = '" + sIcode + "' \n"
					+   " AND SM01.会員ＣＤ = '" + sKcode + "' \n"
					+   " AND SM01.部門ＣＤ = '" + sBcode + "' \n"
					+   " AND SM01.削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[1]  = reader.GetString(0).Trim();
					sRet[2]  = reader.GetString(1).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[3]  = reader.GetString(2).Trim();
					sRet[3]  = reader.GetString(2).TrimEnd(); // 得意先部課名
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
// MOD 2016.02.02 BEVAS）松本 荷送人マスタ取得項目追加（重量、才数） START
					sRet[4]  = reader.GetDecimal(3).ToString("#,##0").Trim(); // 重量
					sRet[5]  = reader.GetDecimal(4).ToString("#,##0").Trim(); // 才数
// MOD 2016.02.02 BEVAS）松本 荷送人マスタ取得項目追加（重量、才数） END

					sRet[0] = "正常終了";
				}else{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.02 東都）伊賀 内容確認処理見直し END

// ADD 2005.06.10 東都）小童谷 出荷日更新 START
		/*********************************************************************
		 * 出荷日更新
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_syukkabi(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷日更新開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
					string cmdQuery 
						= "UPDATE \"ＳＴ０１出荷ジャーナル\" \n"
						+    "SET 出荷日             = '" + sData[4]  +"', \n"
						+        "更新ＰＧ           = '" + sData[5] +"',"
						+        "更新者             = '" + sData[6] +"', \n"
						+        "更新日時           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE 会員ＣＤ           = '" + sData[0]  +"' \n"
						+ "   AND 部門ＣＤ           = '" + sData[1]  +"' \n"
						+ "   AND 登録日             = '" + sData[2] +"' \n"
						+ "   AND \"ジャーナルＮＯ\" = '" + sData[3] +"' \n"
//保留：既存のＰＧにはどう対応するか？
//保留 MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 START
//保留					+ "   AND 送り状番号         = ' ' \n"
//保留 MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 END
						+ "   AND 削除ＦＧ           = '0' \n";

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					tran.Commit();
//保留 MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 START
					if(iUpdRow == 0)
						sRet[0] = "データ編集中に他の端末より更新されています。\r\n再度、最新データを呼び出して更新してください。";
					else
						sRet[0] = "正常終了";
//					sRet[0] = "正常終了";
//保留 MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.10 東都）小童谷 出荷日更新 END

// ADD 2006.07.25 東都）山本 出荷照会の表示項目追加 START
		/*********************************************************************
		 * 出荷一覧取得
		 * 引数：会員ＣＤ、部門ＣＤ、荷受人ＣＤ、荷送人ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日、状態
		 * 戻値：ステータス、一覧（出荷日、住所１、名前１、）...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA1_SELECT_1 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " NVL(COUNT(S.ROWID),0), \n"
			+       " NVL(SUM(S.個数),0), \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//// MOD 2011.04.13 東都）高木 重量入力不可対応 START
////			+       " NVL(SUM(S.重量),0), \n"
////			+       " NVL(SUM(S.才数),0) \n"
//			+       " NVL(SUM(DECODE(S.運賃重量,'     ',0,S.運賃重量)),0), \n"
//			+       " NVL(SUM(DECODE(S.運賃才数,'     ',0,S.運賃才数)),0), \n"
//			+       " 1 \n"
//// MOD 2011.04.13 東都）高木 重量入力不可対応 END
			+       " NVL(SUM(S.重量),0) \n"
			+       ", NVL(SUM(S.才数),0) \n"
			+       ", NVL(SUM(DECODE(S.運賃重量,'     ',0,S.運賃重量)),0) \n"
			+       ", NVL(SUM(DECODE(S.運賃才数,'     ',0,S.運賃才数)),0) \n"
			+       ", NVL(MAX(CM01.保留印刷ＦＧ),'0') \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			+  " FROM \"ＳＴ０１出荷ジャーナル\" S, ＡＭ０３状態 J \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;

		private static string GET_SYUKKA1_SELECT_2 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " SUBSTR(S.出荷日,5,2) || '/' || SUBSTR(S.出荷日,7,2), S.住所１, S.名前１, \n"
			+       " TO_CHAR(S.個数), S.重量, S.輸送指示１, \n"
			+       " S.品名記事１, S.送り状番号, DECODE(S.元着区分,1,'元払',2,'着払',S.元着区分), \n"
			+       " DECODE(S.指定日,0,' ',(SUBSTR(S.指定日,5,2) || '/' || SUBSTR(S.指定日,7,2) || DECODE(S.指定日区分,'0','必着','1','指定',''))), \n"

			+       " DECODE(S.詳細状態,'  ', NVL(J.状態名, S.状態),NVL(J.状態詳細名, S.詳細状態)), \n"
			+       " SUBSTR(S.登録日,5,2) || '/' || SUBSTR(S.登録日,7,2), \n"
			+       " S.お客様出荷番号, TO_CHAR(S.\"ジャーナルＮＯ\"), S.登録日, \n"
			+       " SUBSTR(S.出荷日,1,4) || '/' || SUBSTR(S.出荷日,5,2) || '/' || SUBSTR(S.出荷日,7,2), \n"
			+       " S.登録者, \n"
			+       " S.才数, \n"
// MOD 2007.02.20 東都）高木 保険料の表示 START
//			+       " S.保険金額, \n"
			+       " S.諸料金, \n"
// MOD 2007.02.20 東都）高木 保険料の表示 END
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 START
//			+       " S.運賃 \n"
			+       " S.運賃 + S.中継 \n"
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 END
// ADD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 START
			+       ", DECODE(S.削除ＦＧ,'1','削',' ') \n"
			+       ", DECODE(S.送り状発行済ＦＧ,'1','済',' ') \n"
// ADD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 END
// ADD 2007.07.06 東都）高木 一覧項目に発店ＣＤを表示 START
			+       ", S.発店ＣＤ \n"
// ADD 2007.07.06 東都）高木 一覧項目に発店ＣＤを表示 END
// ADD 2008.10.29 東都）高木 請求先情報を追加 START
			+       ", S.得意先ＣＤ \n"
			+       ", S.部課ＣＤ \n"
			+       ", S.部課名 \n"
// ADD 2008.10.29 東都）高木 請求先情報を追加 END
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+       ", NVL(CM01.記事連携ＦＧ,'0') \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする START
			+       ", S.状態, S.出荷済ＦＧ \n"
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			+       ", S.運賃才数, S.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 START
			+       ", DECODE(S.処理０３,'          ',' ',('20' || SUBSTR(S.処理０３,1,2) || '/' || SUBSTR(S.処理０３,3,2) || '/' || SUBSTR(S.処理０３,5,2) || ' ' || SUBSTR(S.処理０３,7,2) || ':' || SUBSTR(S.処理０３,9,2))) \n"
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" S, ＡＭ０３状態 J \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;

		private static string GET_SYUKKA1_SELECT_2_SORT
			= " ORDER BY 登録日,\"ジャーナルＮＯ\" ";

		private static string GET_SYUKKA1_SELECT_2_SORT2
			= " ORDER BY 出荷日,登録日,\"ジャーナルＮＯ\" ";

		[WebMethod]
		public String[] Get_syukka1(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
		{
			string[] sKey = new string[]{sKCode, sBCode, sTCode, sICode, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_syukka2(sUser, sKey);
		}
		[WebMethod]
		public String[] Get_syukka2(string[] sUser, string[] sKey)
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷一覧取得２開始");
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string sTCode   = sKey[2];
			string sICode   = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s送り状番号開始 = ""; if(sKey.Length >  8) s送り状番号開始 = sKey[ 8];
			string s送り状番号終了 = ""; if(sKey.Length >  9) s送り状番号終了 = sKey[ 9];
			string sお客様番号開始 = ""; if(sKey.Length > 10) sお客様番号開始 = sKey[10];
			string sお客様番号終了 = ""; if(sKey.Length > 11) sお客様番号終了 = sKey[11];
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 START
			string s配完Ｓ出力形式 = "";	if(sKey.Length > 12) s配完Ｓ出力形式 = sKey[12];
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 END

			if(s送り状番号開始.Length == 0) s送り状番号開始 = s送り状番号終了;
			if(s送り状番号終了.Length == 0) s送り状番号終了 = s送り状番号開始;
			if(sお客様番号開始.Length == 0) sお客様番号開始 = sお客様番号終了;
			if(sお客様番号終了.Length == 0) sお客様番号終了 = sお客様番号開始;
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			string s登録件数 = "0";
			string s個数合計 = "0";
			int    i登録件数 = 0;
			decimal d重量合計 = 0;
			decimal d才数合計 = 0;
			string s送り状    = "";
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
			decimal d才数重量合計 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			StringBuilder sbRet = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE S.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND S.部門ＣＤ = '" + sBCode + "' \n");
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 START
				if(s送り状番号開始.Length > 0){
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 START
					if(s送り状番号開始.Length >= 11){
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 END
						if(s送り状番号開始 == s送り状番号終了){
							sbQuery.Append(" AND S.送り状番号 = '0000"+ s送り状番号開始 + "' \n");
						}else{
							sbQuery.Append(" AND S.送り状番号 BETWEEN '0000"+ s送り状番号開始
														 + "' AND '0000"+ s送り状番号終了 + "' \n");
						}
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 START
					}else if(s送り状番号開始.Length >= 6){
						sbQuery.Append(" AND S.送り状番号 BETWEEN '0000"+ s送り状番号開始.PadRight(11,'0')
													 + "' AND '0000"+ s送り状番号終了.PadRight(11,'9') + "' \n");
					}else if(s送り状番号開始.Length == 4
							|| s送り状番号開始.Length == 5){
						if(s送り状番号開始 == s送り状番号終了){
							sbQuery.Append(" AND SUBSTR(S.送り状番号,"
							 +(5+11-s送り状番号開始.Length)+","+s送り状番号開始.Length
							 + ") = '" + s送り状番号開始 + "' \n");
						}else{
							sbQuery.Append(" AND SUBSTR(S.送り状番号,"
							 +(5+11-s送り状番号開始.Length)+","+s送り状番号開始.Length
							 + ") BETWEEN '" + s送り状番号開始
							 + "' AND '"+ s送り状番号終了 + "' \n");
						}
					}else{
					}
// MOD 2011.03.17 東都）高木 送り状番号の桁数チェックの変更 END
				}
				if(sお客様番号開始.Length > 0){
					if(sお客様番号開始 == sお客様番号終了){
						sbQuery.Append(" AND S.お客様出荷番号 = '"+ sお客様番号開始 + "' \n");
					}else{
						sbQuery.Append(" AND S.お客様出荷番号 BETWEEN '"+ sお客様番号開始
													 + "' AND '"+ sお客様番号終了 + "' \n");
					}
				}
// MOD 2009.11.04 東都）高木 検索条件に送り状番号とお客様番号の項目を追加 END

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND S.荷受人ＣＤ = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND S.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND S.荷受人ＣＤ = '"+ sTCode + "' \n");
					sbQuery.Append(" AND S.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(sSday != "0")
				{
					if(iSyuka == 0)
						sbQuery.Append(" AND S.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
					else
						sbQuery.Append(" AND S.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				}
				
				if(sJyoutai != "00")
				{
					if(sJyoutai == "aa")
						sbQuery.Append(" AND S.状態 <> '01' \n");
					else
						sbQuery.Append(" AND S.状態 = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND S.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND S.状態     = J.状態ＣＤ(+) \n");
				sbQuery.Append(" AND S.詳細状態 = J.状態詳細ＣＤ(+) \n");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
				sbQuery.Append(" AND S.会員ＣＤ = CM01.会員ＣＤ(+) \n");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END

				sbQuery2.Append(GET_SYUKKA1_SELECT_1);
				sbQuery2.Append(sbQuery);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery2);

				if(reader.Read())
				{
					s登録件数   = reader.GetDecimal(0).ToString("#,##0").Trim();
					s個数合計   = reader.GetDecimal(1).ToString("#,##0").Trim();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					if(reader.GetString(6) == "1"){
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						d重量合計   = reader.GetDecimal(2);
						d才数合計   = reader.GetDecimal(3);
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}else{
						d重量合計   = reader.GetDecimal(4);
						d才数合計   = reader.GetDecimal(5);
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				sRet[1] = s登録件数;
				sRet[2] = s個数合計;
				d重量合計 = d重量合計 + d才数合計 * 8;
				sRet[3] = d重量合計.ToString("#,##0").Trim();

				i登録件数 = int.Parse(s登録件数.Replace(",",""));

				if(i登録件数 == 0)
				{
					sRet[0] = "該当データがありません";
				}
				else
				{
					sRet = new string[i登録件数 + 4];
					sRet[0] = "正常終了";
					sRet[1] = s登録件数;
					sRet[2] = s個数合計;
					sRet[3] = d重量合計.ToString("#,##0").Trim();

					sbQuery2 = new StringBuilder(1024);
					if(iSyuka == 0)
					{
						sbQuery2.Append(GET_SYUKKA1_SELECT_2);
						sbQuery2.Append(sbQuery);
						sbQuery2.Append(GET_SYUKKA1_SELECT_2_SORT2);
						reader = CmdSelect(sUser, conn2, sbQuery2);
					}
					else
					{
						sbQuery2.Append(GET_SYUKKA1_SELECT_2);
						sbQuery2.Append(sbQuery);
						sbQuery2.Append(GET_SYUKKA1_SELECT_2_SORT);
						reader = CmdSelect(sUser, conn2, sbQuery2);
					}

					int iCnt = 4;
					while (reader.Read() && iCnt < sRet.Length)
					{
						sbRet = new StringBuilder(1024);

// ADD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 START
						sbRet.Append(sSepa + reader.GetString(20));			// 削除ＦＧ
						sbRet.Append(sSepa + reader.GetString(21));			// 送り状発行済ＦＧ
// ADD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 END
						sbRet.Append(sSepa + reader.GetString(0));			// 出荷日
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//						sbRet.Append(sSepa + reader.GetString(1).Trim());	// 住所１
//						sbRet.Append(sCRLF + reader.GetString(2).Trim());	// 名前１
						sbRet.Append(sSepa + reader.GetString(1).TrimEnd()); // 住所１
						sbRet.Append(sCRLF + reader.GetString(2).TrimEnd()); // 名前１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
						sbRet.Append(sSepa + reader.GetString(3));			// 個数
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//						d才数合計 = reader.GetDecimal(17);
//						d才数合計 = d才数合計 * 8;
//						if(d才数合計 == 0)
//							sbRet.Append(sSepa + reader.GetDecimal(4).ToString("#,##0").Trim()); // 重量
//						else
//							sbRet.Append(sSepa + d才数合計.ToString("#,##0").Trim());		// 才数
						s運賃才数 = reader.GetString(29).TrimEnd();
						s運賃重量 = reader.GetString(30).TrimEnd();
						d才数重量 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
						s重量入力制御 = reader.GetString(31).TrimEnd();
						if(s重量入力制御 == "1" 
						&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//						&& (reader.GetDecimal(4) != 0 || reader.GetDecimal(17) != 0)
						){
							d才数重量 = reader.GetDecimal(17) * 8;	// 才数
							d才数重量 += reader.GetDecimal(4);		// 重量
							sbRet.Append(sSepa + d才数重量.ToString("#,##0").Trim());
						}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
							if(s運賃才数.Length > 0){
								try{
									d才数重量 += (Decimal.Parse(s運賃才数) * 8);
								}catch(Exception){}
							}
							if(s運賃重量.Length > 0){
								try{
									d才数重量 += Decimal.Parse(s運賃重量);
								}catch(Exception){}
							}
							if(d才数重量 == 0){
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//保留								if(s重量入力制御 == "1"){
//保留									sbRet.Append(sSepa + "0");
//保留								}else{
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
									sbRet.Append(sSepa + " ");
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//保留								}
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
							}else{
								sbRet.Append(sSepa + d才数重量.ToString("#,##0").Trim());
							}
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
						}
						d才数重量合計 += d才数重量;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 START
//						sbRet.Append(sSepa + reader.GetDecimal(18).ToString("#,##0").Trim());
//																			// 保険料
//						sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
//																			// 運賃
//
//						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());		// 輸送指示１
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());		// 品名記事１
//						s送り状 = reader.GetString(7).Trim();              		// 送り状番号
//						if(s送り状.Length == 0)
//							sbRet.Append(sSepa + s送り状);
//						else
//							sbRet.Append(sSepa + s送り状.Remove(0,4));
//						sbRet.Append(sCRLF + reader.GetString(8));			// 元着区分
//						sbRet.Append(sSepa + reader.GetString(9));			// 指定日
//						sbRet.Append(sSepa + reader.GetString(10).Trim());	// 状態
//						sbRet.Append(sSepa + reader.GetString(11));			// 登録日
//						sbRet.Append(sSepa + reader.GetString(12).Trim());	// お客様出荷番号
						s送り状 = reader.GetString(7).Trim();              		// 送り状番号
						if(s送り状.Length == 0)
							sbRet.Append(sSepa + s送り状);
						else
							sbRet.Append(sSepa + s送り状.Remove(0,4));
						sbRet.Append(sCRLF + reader.GetString(8));			// 元着区分
// ADD 2007.07.06 東都）高木 一覧項目に発店ＣＤを表示 START
						sbRet.Append("　" + reader.GetString(22));			// 発店ＣＤ
// ADD 2007.07.06 東都）高木 一覧項目に発店ＣＤを表示 END
						sbRet.Append(sSepa + reader.GetString(12).Trim());	// お客様出荷番号
						sbRet.Append(sSepa + reader.GetString(9));			// 指定日
						sbRet.Append(sSepa + reader.GetString(10).Trim());	// 状態
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 START
						if(s配完Ｓ出力形式.Equals("1"))
						{
							sbRet.Append(sSepa + reader.GetString(32).Trim());// 配完日付・時刻
						}
// MOD 2013.10.07 BEVAS）高杉 配完日付・時刻を追加 END
						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());// 輸送指示１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());	// 品名記事１
						sbRet.Append(sCRLF + reader.GetString(6).TrimEnd()); // 品名記事１
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
// MOD 2008.06.17 東都）高木 運賃が０の場合[＊]表示 START
//						sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
																			// 運賃
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
//						if(reader.GetDecimal(19) == 0){
						if(reader.GetDecimal(19) == 0 || reader.GetString(26).Equals("1")){
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
							sbRet.Append(sSepa + "*");
						}else{
							sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
						}
// MOD 2008.06.17 東都）高木 運賃が０の場合[＊]表示 END
						sbRet.Append(sSepa + reader.GetDecimal(18).ToString("#,##0").Trim());
																			// 保険料
						sbRet.Append(sSepa + reader.GetString(11));			// 登録日
// MOD 2007.01.17 東都）高木 一覧項目に削除ＦＧ、送り状発行済ＦＧを表示 END
						sbRet.Append(sSepa + reader.GetString(13));			// ジャーナルＮＯ
						sbRet.Append(sSepa + reader.GetString(14));			// 登録日
						sbRet.Append(sSepa + reader.GetString(15));			// 出荷日
						sbRet.Append(sSepa + reader.GetString(16));			// 登録者
// ADD 2008.10.29 東都）高木 請求先情報を追加 START
						sbRet.Append(sSepa + reader.GetString(23));			// 請求先ＣＤ（得意先ＣＤ）
						sbRet.Append(sSepa + reader.GetString(24));			// 請求先部課ＣＤ
						sbRet.Append(sSepa + reader.GetString(25));			// 請求先部課名
// ADD 2008.10.29 東都）高木 請求先情報を追加 END
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする START
						sbRet.Append(sSepa + reader.GetString(27));			// 状態
						sbRet.Append(sSepa + reader.GetString(28));			// 出荷済ＦＧ
// MOD 2010.11.12 東都）高木 未発行データを削除可能にする END
						sbRet.Append(sSepa);
						sRet[iCnt] = sbRet.ToString();
						iCnt++;
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					sRet[3] = d才数重量合計.ToString("#,##0").Trim();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				}

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2006.07.25 東都）山本 出荷照会の表示項目追加 END
// ADD 2006.07.27 東都）山本 出荷照会の表示項目追加(CSV出力) START
		/*********************************************************************
		 * 出荷一覧取得（ＣＳＶ出力用）
		 * 引数：会員ＣＤ、部門ＣＤ、荷受人ＣＤ、荷送人ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日、状態
		 * 戻値：ステータス、登録日、ジャーナルＮＯ、荷受人ＣＤ...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA1_SELECT_3
			= "SELECT /*+ INDEX(J ST01IDX2) INDEX(N SM01PKEY) */ \n"
			+       " J.登録日, J.出荷日, 送り状番号, J.荷受人ＣＤ, J.郵便番号, \n"
			+       " '(' || TRIM(J.電話番号１) || ')' || TRIM(J.電話番号２) || '-' || J.電話番号３, \n"
			+       " J.住所１, J.住所２, J.住所３, J.名前１, J.名前２, J.特殊計, J.着店ＣＤ, J.着店名, \n"
			+       " J.荷送人ＣＤ, NVL(N.郵便番号, ' '), \n"
			+       " NVL(N.電話番号１,' '), NVL(N.電話番号２,' '), NVL(N.電話番号３,' '), \n"
			+       " NVL(N.住所１,' '), NVL(N.住所２,' '), NVL(N.名前１,' '), NVL(N.名前２,' '), \n"
			+       " TO_CHAR(J.個数), TO_CHAR(J.重量), \n"
			+       " J.指定日, J.輸送指示１, J.輸送指示２, J.品名記事１, J.品名記事２, J.品名記事３, \n"
			+       " J.元着区分, TO_CHAR(J.保険金額), J.お客様出荷番号, \n"
			+       " J.得意先ＣＤ, J.部課ＣＤ, J.才数 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 START
//			+       " , TO_CHAR(J.運賃) \n"
			+       " , TO_CHAR(J.運賃 + J.中継) \n"
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 END
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+       " , NVL(CM01.記事連携ＦＧ,'0') \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
			+       " , J.運賃才数, J.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" J,ＳＭ０１荷送人 N \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;

		[WebMethod]
		public String[] Get_csvwrite1(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai, string sIraiCd)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "ＣＳＶ出力１開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//			decimal d才数 = 0;
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.部門ＣＤ = '" + sBCode + "' \n");

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND J.荷受人ＣＤ = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND J.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND J.荷受人ＣＤ = '"+ sTCode + "' \n");
					sbQuery.Append(" AND J.荷送人ＣＤ = '"+ sICode + "' \n");
				}
				if(iSyuka == 0)
					sbQuery.Append(" AND J.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sIraiCd.Trim().Length != 0)
				{
					sbQuery.Append(" AND '" + sIraiCd + "' = J.荷送人ＣＤ(+) \n");
				}

				if(sJyoutai != "00")
				{
// ADD 2008.07.09 東都）高木 未発行分を除外する START
					if(sJyoutai == "aa")
						sbQuery.Append(" AND J.状態 <> '01' \n");
					else
// ADD 2008.07.09 東都）高木 未発行分を除外する END
						sbQuery.Append(" AND J.状態 = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND J.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND J.荷送人ＣＤ     = N.荷送人ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.会員ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.部門ＣＤ(+) \n");
				sbQuery.Append(" AND '0' = N.削除ＦＧ(+) ");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
				sbQuery.Append(" AND J.会員ＣＤ = CM01.会員ＣＤ(+) \n");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END

				OracleDataReader reader;
				if(iSyuka == 0)
				{
					sbQuery2.Append(GET_SYUKKA1_SELECT_3);
					sbQuery2.Append(sbQuery);
					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
					reader = CmdSelect(sUser, conn2, sbQuery2);
				}
				else
				{
					sbQuery2.Append(GET_SYUKKA1_SELECT_3);
					sbQuery2.Append(sbQuery);
					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
					reader = CmdSelect(sUser, conn2, sbQuery2);
				}

				StringBuilder sbData = new StringBuilder(1024);
				while (reader.Read())
				{
					sbData = new StringBuilder(1024);
					sbData.Append(sDbl + reader.GetString(0).Trim() + sDbl);					// 登録日
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl       );	// 出荷日
					string sNo = reader.GetString(2).Trim();									// 送り状番号(XXX-XXXX-XXXX)
					if(sNo.Length == 15)
					{
						sbData.Append(sKanma + sDbl + sNo.Substring(4,3)
							+ "-" + sNo.Substring(7,4) + "-" + sNo.Substring(11) + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl);	// 荷受人ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// 荷受人郵便番号
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// 荷受人電話番号
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// 荷受人住所１
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// 荷受人住所２
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// 荷受人住所３
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// 荷受人名前１
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);			// 荷受人名前２
					sbData.Append(sKanma + sDbl + reader.GetString(6).TrimEnd() + sDbl);  // 荷受人住所１
					sbData.Append(sKanma + sDbl + reader.GetString(7).TrimEnd() + sDbl);  // 荷受人住所２
					sbData.Append(sKanma + sDbl + reader.GetString(8).TrimEnd() + sDbl);  // 荷受人住所３
					sbData.Append(sKanma + sDbl + reader.GetString(9).TrimEnd() + sDbl);  // 荷受人名前１
					sbData.Append(sKanma + sDbl + reader.GetString(10).TrimEnd() + sDbl); // 荷受人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// 特殊計
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// 着店ＣＤ
					sbData.Append(sKanma + sDbl + reader.GetString(13).Trim() + sDbl       );	// 着店名
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// 荷送人ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(15).Trim() + sDbl);	// 荷送人郵便番号

					string sTel = reader.GetString(16).Trim();									// 荷送人電話番号
					if(sTel.Length != 0)
					{
						sbData.Append(sKanma + sDbl + "(" + sTel + ")"
							+ "-" + reader.GetString(17).Trim() + "-" + reader.GetString(18).Trim() + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sbData.Append(sKanma + sDbl + reader.GetString(19).Trim() + sDbl);			// 荷送人住所１
//					sbData.Append(sKanma + sDbl + reader.GetString(20).Trim() + sDbl);			// 荷送人住所２
//					sbData.Append(sKanma + sDbl + reader.GetString(21).Trim() + sDbl);			// 荷送人名前１
//					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl);			// 荷送人名前２
					sbData.Append(sKanma + sDbl + reader.GetString(19).TrimEnd() + sDbl); // 荷送人住所１
					sbData.Append(sKanma + sDbl + reader.GetString(20).TrimEnd() + sDbl); // 荷送人住所２
					sbData.Append(sKanma + sDbl + reader.GetString(21).TrimEnd() + sDbl); // 荷送人名前１
					sbData.Append(sKanma + sDbl + reader.GetString(22).TrimEnd() + sDbl); // 荷送人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sbData.Append(sKanma + reader.GetString(23)                            );	// 個数
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					d才数 = reader.GetDecimal(36);													// 才数
//					d才数 = d才数 * 8;
//					if(d才数 == 0)
//						sbData.Append(sKanma + reader.GetString(24)                            );	// 重量
//					else
//						sbData.Append(sKanma + d才数.ToString()                            );
					s運賃才数 = reader.GetString(39).TrimEnd();
					s運賃重量 = reader.GetString(40).TrimEnd();
					d才数重量 = 0;
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(41).TrimEnd();
					if(s重量入力制御 == "1"
					&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						d才数重量 += (reader.GetDecimal(36) * 8);		// 才数
						if(reader.GetString(24).TrimEnd().Length > 0){	// 重量
							try{
								d才数重量 += Decimal.Parse(reader.GetString(24).TrimEnd());
							}catch(Exception){}
						}
						sbData.Append(sKanma + d才数重量.ToString());	// 重量
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						if(s運賃才数.Length > 0){
							try{
								d才数重量 += (Decimal.Parse(s運賃才数) * 8);
							}catch(Exception){}
						}
						if(s運賃重量.Length > 0){
							try{
								d才数重量 += Decimal.Parse(s運賃重量);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d才数重量.ToString());		// 重量
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

					if(reader.GetString(25).Trim() == "0")
						sbData.Append(sKanma + sDbl + sDbl);										// 指定日
					else
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// 指定日
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// 輸送指示１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// 輸送指示２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// 品名記事１
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// 品名記事２
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// 品名記事３
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// 元着区分
					sbData.Append(sKanma + reader.GetString(32)                            );	// 保険金額
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// お客様出荷番号
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(34).Trim() + sDbl);	// 得意先ＣＤ
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(35).Trim() + sDbl);	// 部課ＣＤ
// MOD 2008.06.17 東都）高木 運賃が０の場合[＊]表示 START
//					sbData.Append(sKanma + reader.GetString(37)                            );	// 運賃
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
//					if(reader.GetString(37).Trim().Equals("0")){
					if(reader.GetString(37).Trim().Equals("0") || reader.GetString(38).Equals("1")){
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
						sbData.Append(sKanma + "*");
					}else{
						sbData.Append(sKanma + reader.GetString(37));
					}
// MOD 2008.06.17 東都）高木 運賃が０の場合[＊]表示 END
					sList.Add(sbData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}
// ADD 2006.07.27 東都）山本 出荷照会の表示項目追加(CSV出力) END
// ADD 2007.04.20 東都）高木 ＣＳＶエントリ（自動出荷登録）の高速化 START
		/*********************************************************************
		 * ＣＳＶエントリ（自動出荷登録）チェック１
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、出荷日
		 *
		 * 元：sv_init.Get_syukabi(gsaユーザ,gs会員ＣＤ, gs部門ＣＤ);
		 * 元：sv_syukka.Get_hatuten2(gsaユーザ,gs会員ＣＤ,gs部門ＣＤ);
		 * 元：sv_syukka.Get_syuuyakuten2(gsaユーザ,gs会員ＣＤ,gs部門ＣＤ);
		 *
		 *********************************************************************/
		private static string CHECK_AUTOENTRY1_SELECT_1
			= "SELECT /*+ INDEX(CM02 CM02PKEY) INDEX(CM14 CM14PKEY) INDEX(CM10 CM10PKEY) */ \n"
			+ " CM02.出荷日, CM02.郵便番号, CM14.店所ＣＤ, CM10.店所名, CM10.集約店ＣＤ \n"
			+ " FROM ＣＭ０２部門 CM02, \n"
			+ " ＣＭ１４郵便番号 CM14, \n"
			+ " ＣＭ１０店所 CM10 \n"
			;
		private static string CHECK_AUTOENTRY1_WHERE_1
			= " AND CM02.削除ＦＧ = '0' \n"
			+ " AND CM02.郵便番号 = CM14.郵便番号 \n"
			+ " AND CM14.店所ＣＤ = CM10.店所ＣＤ \n"
			+ " AND CM10.削除ＦＧ = '0' \n"
			;
		[WebMethod]
		public String[] Check_autoEntry1(string[] sUser, string sKCode, string sBCode)
		{
			logWriter(sUser, INF, "ＣＳＶエントリ（自動出荷登録）チェック１");

			OracleConnection conn2 = null;
			String[] sRet = new string[10];
			sRet[0] = "正常終了";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(CHECK_AUTOENTRY1_SELECT_1);
				sbQuery.Append(" WHERE CM02.会員ＣＤ = '"+ sKCode + "' \n");
				sbQuery.Append(" AND CM02.部門ＣＤ = '"+ sBCode + "' \n");
				sbQuery.Append(CHECK_AUTOENTRY1_WHERE_1);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
					sRet[5] = reader.GetString(4).Trim();
				}else{
					sRet[0] = "該当データがありません";
				}

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				sbQuery = null;
				disposeReader(reader);
				reader = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
		
		/*********************************************************************
		 * 届け先情報取得２
		 * 引数：会員ＣＤ、部門ＣＤ、荷受人ＣＤ
		 * 戻値：ステータス、登録日、ジャーナルＮＯ、荷受人ＣＤ...
		 *
		 * 元：sv_otodoke.Get_Stodokesaki(gsaユーザ,gs会員ＣＤ,gs部門ＣＤ,sKey[4]);
		 *
		 *********************************************************************/
		private static string GET_STODOKESAKI2_SELECT
			= "SELECT /*+ INDEX(SM02 SM02PKEY) */ \n"
			+ " SM02.電話番号１, SM02.電話番号２, SM02.電話番号３ \n"
			+ ", SM02.住所１, SM02.住所２, SM02.住所３ \n"
			+ ", SM02.名前１, SM02.名前２, SM02.郵便番号, SM02.特殊計 \n"
			+ " FROM ＳＭ０２荷受人 SM02 \n"
			;
		private static string GET_STODOKESAKI2_WHERE
			= " AND SM02.削除ＦＧ = '0' \n"
			;

		[WebMethod]
		public String[] Get_Stodokesaki2(string[] sUser, string sKCode, string sBCode, string sTCode, string sYCode)
		{
			logWriter(sUser, INF, "届け先情報取得２開始");
			OracleConnection conn2 = null;
			String[] sRet = new string[15];
			sRet[0] = "正常終了";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_STODOKESAKI2_SELECT);
				sbQuery.Append(" WHERE SM02.会員ＣＤ = '"+ sKCode + "' \n");
				sbQuery.Append(" AND SM02.部門ＣＤ = '"+ sBCode + "' \n");
				sbQuery.Append(" AND SM02.荷受人ＣＤ = '" + sTCode + "' \n");
				sbQuery.Append(GET_STODOKESAKI2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();	//電話番号１
					sRet[2] = reader.GetString(1).Trim();	//電話番号２
					sRet[3] = reader.GetString(2).Trim();	//電話番号３
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[4] = reader.GetString(3).Trim();	//住所１
//					sRet[5] = reader.GetString(4).Trim();	//住所２
//					sRet[6] = reader.GetString(5).Trim();	//住所３
//					sRet[7] = reader.GetString(6).Trim();	//名前１
//					sRet[8] = reader.GetString(7).Trim();	//名前２
					sRet[4] = reader.GetString(3).TrimEnd(); // 住所１
					sRet[5] = reader.GetString(4).TrimEnd(); // 住所２
					sRet[6] = reader.GetString(5).TrimEnd(); // 住所３
					sRet[7] = reader.GetString(6).TrimEnd(); // 名前１
					sRet[8] = reader.GetString(7).TrimEnd(); // 名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sRet[9] = reader.GetString(8).Trim();	//郵便番号
					sRet[10] = reader.GetString(9).Trim();	//特殊計

					//郵便番号が未入力で、かつ、荷受人の郵便番号が入力されている場合
					if(sYCode.Length == 0 && sRet[9].Length > 0){
						sbQuery = null;
						disposeReader(reader);
						reader = null;

						//郵便番号検索（着店の決定）
						sbQuery = new StringBuilder(1024);
						sbQuery.Append(GET_AUTOENTRYPREF2_SELECT);
						sbQuery.Append(" WHERE CM14.郵便番号 = '" + sRet[9] + "' \n");
						sbQuery.Append(GET_AUTOENTRYPREF2_WHERE);

						reader = CmdSelect(sUser, conn2, sbQuery);
						if (reader.Read()){
							sRet[11] = reader.GetString(0).Trim()	// 都道府県ＣＤ
									+ reader.GetString(1).Trim()	// 市区町村ＣＤ
									+ reader.GetString(2).Trim();	// 大字通称ＣＤ
							sRet[12] = reader.GetString(3).Trim();	// 店所ＣＤ
							sRet[13] = reader.GetString(4).Trim();	// 店所名
						}else{
//							sRet[0] = "該当データがありません";
							sRet[0] = "2";
						}
					}else{
						sRet[11] = "";
						sRet[12] = "";
						sRet[13] = "";
					}
				}else{
//					sRet[0] = "該当データがありません";
					sRet[0] = "1";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "↑届け先情報取得２");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "届け先情報取得２");
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				sbQuery = null;
				disposeReader(reader);
				reader = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * 着店情報取得２
		 * 引数：郵便番号
		 * 戻値：ステータス、住所ＣＤ、店所ＣＤ、店所名
		 *
		 * 元：sv_syukka.Get_autoEntryPref(gsaユーザ,sKey[14]);
		 *
		 *********************************************************************/
		private static string GET_AUTOENTRYPREF2_SELECT
			= "SELECT /*+ INDEX(CM14 CM14PKEY) INDEX(CM10 CM10PKEY) */ \n"
			+ " CM14.都道府県ＣＤ, CM14.市区町村ＣＤ, CM14.大字通称ＣＤ \n"
			+ ", NVL(CM10.店所ＣＤ,' '), NVL(CM10.店所名,' ') \n"
			+ " FROM ＣＭ１４郵便番号 CM14 \n"
			+ ", ＣＭ１０店所 CM10 \n"
			;
		private static string GET_AUTOENTRYPREF2_WHERE
			= " AND CM14.店所ＣＤ = CM10.店所ＣＤ(+) \n"
			+ " AND '0' = CM10.削除ＦＧ(+) \n"
			;

		[WebMethod]
		public String[] Get_autoEntryPref2(string[] sUser, string sYCode)
		{
//			logWriter(sUser, INF, "着店情報取得２開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];
			sRet[0] = "正常終了";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_AUTOENTRYPREF2_SELECT);
				sbQuery.Append(" WHERE CM14.郵便番号 = '" + sYCode + "' \n");
				sbQuery.Append(GET_AUTOENTRYPREF2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim()	// 都道府県ＣＤ
							+ reader.GetString(1).Trim()	// 市区町村ＣＤ
							+ reader.GetString(2).Trim();	// 大字通称ＣＤ
					sRet[2] = reader.GetString(3).Trim();	// 店所ＣＤ
					sRet[3] = reader.GetString(4).Trim();	// 店所名
				}else{
					sRet[0] = "該当データがありません";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "↑着店情報取得２");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "着店情報取得２");
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				sbQuery = null;
				disposeReader(reader);
				reader = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * 依頼主情報取得２
		 * 引数：会員ＣＤ、部門ＣＤ、部門郵便番号、荷送人ＣＤ
		 * 戻値：ステータス、得意先ＣＤ、得意先部課ＣＤ、得意先部課名
		 *
		 * 元：sv_syukka.Get_autoEntryClaim(gsaユーザ,gs会員ＣＤ,gs部門ＣＤ,sKey[18]);
		 *
		 *********************************************************************/
		private static string GET_AUTOENTRYCLAIM2_SELECT
			= "SELECT /*+ INDEX(SM01 SM01PKEY) INDEX(SM04 SM04PKEY) */ \n"
			+ " SM01.得意先ＣＤ, SM01.得意先部課ＣＤ, NVL(SM04.得意先部課名,' ') \n"
			+ " FROM ＳＭ０１荷送人 SM01 \n"
			+ ", ＳＭ０４請求先 SM04 \n"
			;
		private static string GET_AUTOENTRYCLAIM2_WHERE
			= " AND SM01.削除ＦＧ = '0' \n"
			+ " AND SM01.得意先ＣＤ = SM04.得意先ＣＤ(+) \n"
			+ " AND SM01.得意先部課ＣＤ = SM04.得意先部課ＣＤ(+) \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
			+ " AND SM01.会員ＣＤ = SM04.会員ＣＤ(+) \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
			+ " AND '0' = SM04.削除ＦＧ(+) \n"
			;

		[WebMethod]
		public String[] Get_autoEntryClaim2(string[] sUser, string sKCode, string sBCode
															, string sBYCode, string sICode)
		{
//			logWriter(sUser, INF, "依頼主情報取得２開始");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];
			sRet[0] = "正常終了";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_AUTOENTRYCLAIM2_SELECT);
				sbQuery.Append(" WHERE SM01.会員ＣＤ = '"+ sKCode + "' \n");
				sbQuery.Append(" AND SM01.部門ＣＤ = '"+ sBCode + "' \n");
				sbQuery.Append(" AND SM01.荷送人ＣＤ = '" + sICode + "' \n");
				sbQuery.Append(" AND '" + sBYCode + "' = SM04.郵便番号(+) \n");
				sbQuery.Append(GET_AUTOENTRYCLAIM2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();	//得意先ＣＤ
					sRet[2] = reader.GetString(1).Trim();	//得意先部課ＣＤ
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[3] = reader.GetString(2).Trim();	//得意先部課名
					sRet[3] = reader.GetString(2).TrimEnd(); // 得意先部課名
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
				}else{
					sRet[0] = "該当データがありません";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "↑依頼主情報取得２");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "依頼主情報取得２");
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				sbQuery = null;
				disposeReader(reader);
				reader = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2007.04.20 東都）高木 ＣＳＶエントリ（自動出荷登録）の高速化 END
// ADD 2007.04.27 東都）高木 ORA-01000 対応 START
		/*********************************************************************
		 * ジャーナルＮＯ取得
		 * 引数：会員ＣＤ、部門ＣＤ、更新ＰＧ
		 * 戻値：ステータス、登録日、ジャーナルＮＯ
		 *
		 * ORA-01000: maximum open cursors exceeded 対応
		 *
		 *********************************************************************/
		private static string GET_JURNALNO_SELECT
			= "SELECT \"ジャーナルＮＯ登録日\" \n"
			+ ", \"ジャーナルＮＯ管理\" \n"
			+ ", TO_CHAR(SYSDATE,'YYYYMMDD') \n"
			+ " FROM ＣＭ０２部門 \n"
			;
		private static string GET_JURNALNO_SELECT_WHERE
			= " AND 削除ＦＧ = '0' \n"
			+ " FOR UPDATE \n"
			;
		private static string GET_JURNALNO_UPDATE
			= "UPDATE ＣＭ０２部門 SET \n"
			+ " 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
			;
		private static string GET_JURNALNO_UPDATE_WHERE
			= " AND 削除ＦＧ = '0' \n"
			;
		private String[] Get_JurnalNo(string[] sUser, string sKCode, string sBCode, string sPGName)
		{
//			logWriter(sUser, INF, "ジャーナルＮＯ取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[3];
			sRet[0] = "正常終了";

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			string s登録日 = "";
			int i管理ＮＯ  = 0;
			string s日付   = "";

			OracleTransaction tran;
			tran = conn2.BeginTransaction();
			OracleDataReader reader = null;
			StringBuilder sbQuery;
			try
			{
				//ジャーナルＮＯ取得
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_JURNALNO_SELECT);
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append(" AND 部門ＣＤ = '" + sBCode + "' \n");
				sbQuery.Append(GET_JURNALNO_SELECT_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s登録日   = reader.GetString(0).Trim();
					i管理ＮＯ = reader.GetInt32(1);
					s日付     = reader.GetString(2).Trim();

					if(s登録日 == s日付)
					{
						i管理ＮＯ++;
					}
					else
					{
						s登録日 = s日付;
						i管理ＮＯ = 1;
					}
					if(i管理ＮＯ <= 9999)
					{
						sbQuery = new StringBuilder(1024);
						sbQuery.Append(GET_JURNALNO_UPDATE);
						sbQuery.Append(", \"ジャーナルＮＯ登録日\" = '" + s登録日 + "' \n");
						sbQuery.Append(", \"ジャーナルＮＯ管理\" = " + i管理ＮＯ +" \n");
						sbQuery.Append(", 更新ＰＧ = '" + sPGName +"' \n");
						sbQuery.Append(", 更新者   = '" + sUser[1] +"' \n");
						sbQuery.Append(" WHERE 会員ＣＤ = '" + sKCode + "' \n");
						sbQuery.Append(" AND 部門ＣＤ = '" + sBCode + "' \n");
						sbQuery.Append(GET_JURNALNO_UPDATE_WHERE);

						int iUpdRow = CmdUpdate(sUser, conn2, sbQuery);
					}
					else
					{
						sRet[0] = "ジャーナルＮＯの上限を超えました";
					}

					sRet[1] = s登録日;
					sRet[2] = i管理ＮＯ.ToString();
				}else{
					sRet[0] = "該当データがありません";
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				tran.Commit();
//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "↑ジャーナルＮＯ取得");
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "ジャーナルＮＯ取得");
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disposeReader(reader);
				reader = null;
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// ADD 2007.04.27 東都）高木 ORA-01000 対応 END

// ADD 2008.06.12 kcl)森本 着店コード検索方法の変更 START
		/*********************************************************************
		 * 着店取得
		 * 　　ＳＭ０２荷受人、ＣＭ１４郵便番号、ＣＭ１５着店非表示、ＣＭ１９郵便住所
		 *     の４マスタを使用して着店コードを決定する。
		 * 引数：会員コード、部門コード、荷受人コード、郵便番号、住所、氏名
		 * 戻値：ステータス、店所ＣＤ、店所名、住所ＣＤ
		 *
		 * Create : 2008.06.12 kcl)森本
		 * 　　　　　　Get_tyakuten を元に作成
		 * Modify : 2008.12.24 kcl)森本
		 * 　　　　　　ＣＭ１９の検索方法を変更、および氏名からの検索を追加
		 *********************************************************************/
// MOD 2008.12.25 kcl)森本 着店コードの検索方法の再変更 START
//		private String[] Get_tyakuten3(string[] sUser, OracleConnection conn2, 
//			string sKaiinCode, string sBumonCode, string sNiukeCode, 
//			string sYuubin, string sJuusyo)
		private String[] Get_tyakuten3(string[] sUser, OracleConnection conn2, 
			string sKaiinCode, string sBumonCode, string sNiukeCode, 
			string sYuubin, string sJuusyo, string sShimei)
// MOD 2008.12.25 kcl)森本 着店コードの検索方法の再変更 END
		{
			string [] sRet = new string [4];		// 戻り値
			string cmdQuery;						// SQL文
			OracleDataReader reader;
			string tenCD       = string.Empty;		// 店所コード
			string tenName     = string.Empty;		// 店所名
			string juusyoCD    = string.Empty;		// 住所コード
			string address     = string.Empty;		// 住所
			string niuJuusyoCD = string.Empty;		// 荷受人マスタの住所コード
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			///
			/// ＜第１段階＞
			/// 荷受人マスタの着店コードを検索
			/// 
			string niuCode = sNiukeCode.Trim();
			if (niuCode.Length > 0) 
			{
				// SQL文
				cmdQuery
					= "SELECT SM02.特殊ＣＤ, NVL(CM10.店所名, ' '), SM02.住所ＣＤ \n"
					+ "  FROM ＳＭ０２荷受人 SM02 \n"
					+ "  LEFT OUTER JOIN ＣＭ１０店所 CM10 \n"
					+ "    ON SM02.特殊ＣＤ   = CM10.店所ＣＤ \n"
					+ "   AND CM10.削除ＦＧ   = '0' \n"
					+ " WHERE SM02.会員ＣＤ   = '" + sKaiinCode + "' \n"
					+ "   AND SM02.部門ＣＤ   = '" + sBumonCode + "' \n"
					+ "   AND SM02.荷受人ＣＤ = '" + sNiukeCode + "' \n"
					+ "   AND ( LENGTH(TRIM(SM02.特殊ＣＤ)) > 0 \n"
					+ "      OR LENGTH(TRIM(SM02.住所ＣＤ)) > 0 ) \n"
					+ "   AND SM02.削除ＦＧ   = '0' \n";

				// SQL実行
				// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
				//reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

				cmdQuery
					= "SELECT SM02.特殊ＣＤ, NVL(CM10.店所名, ' '), SM02.住所ＣＤ \n"
					+ "  FROM ＳＭ０２荷受人 SM02 \n"
					+ "  LEFT OUTER JOIN ＣＭ１０店所 CM10 \n"
					+ "    ON SM02.特殊ＣＤ   = CM10.店所ＣＤ \n"
					+ "   AND CM10.削除ＦＧ   = '0' \n"
					+ " WHERE SM02.会員ＣＤ   = :p_KaiinCD \n"
					+ "   AND SM02.部門ＣＤ   = :p_BumonCD \n"
					+ "   AND SM02.荷受人ＣＤ = :p_NiukeCD \n"
					+ "   AND ( LENGTH(TRIM(SM02.特殊ＣＤ)) > 0 \n"
					+ "      OR LENGTH(TRIM(SM02.住所ＣＤ)) > 0 ) \n"
					+ "   AND SM02.削除ＦＧ   = '0' \n";

				wk_opOraParam = new OracleParameter[3];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKaiinCode, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBumonCode, ParameterDirection.Input);
				wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sNiukeCode, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

				// データ取得
				if (reader.Read())
				{
					// 該当データあり

					// データ取得
					tenCD    = reader.GetString(0).Trim();		// 店所コード
					tenName  = reader.GetString(1).Trim();		// 店所名
					juusyoCD = reader.GetString(2).Trim();		// 住所コード

					if (tenCD.Length > 0) 
					{
						// 荷受人マスタの着店コードが入力されている場合

						// 住所コードの設定
						if (juusyoCD.Length == 0) 
						{
							// 荷受人マスタの住所コードが空欄の場合

							// 郵便番号マスタから取得
							string [] sResult = this.Get_juusyoCode(sUser, conn2, sYuubin);
							if (sResult[0] == " ") 
								juusyoCD = sResult[1];
						}

						// 戻り値をセット
						sRet[0] = " ";
						sRet[1] = tenCD;
						sRet[2] = tenName;
						sRet[3] = juusyoCD;

						// 終了処理
						disposeReader(reader);
						reader = null;
					
						return sRet;
					} 
					else
					{
						// 荷受人マスタに住所コードのみが入力されている場合

						// 荷受人マスタの住所コードをとっておく
						niuJuusyoCD = juusyoCD;
					}
				}

				// 終了処理
				disposeReader(reader);
				reader = null;
			}

			///
			/// ＜第２段階＞
			/// 郵便番号マスタから着店コードを検索
			///
// ADD 2008.10.31 東都）高木 着店コード検索時に着店非表示チェックを追加 START
			cmdQuery
				= "SELECT CM15.郵便番号 \n"
				+ " FROM ＣＭ１５着店非表示 CM15 \n"
				+ " WHERE CM15.郵便番号 = '" + sYuubin + "' \n"
				+ "   AND CM15.削除ＦＧ = '0' \n";

			// SQL実行
			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery
				= "SELECT CM15.郵便番号 \n"
				+ " FROM ＣＭ１５着店非表示 CM15 \n"
				+ " WHERE CM15.郵便番号 = :p_YuubinNo \n"
				+ "   AND CM15.削除ＦＧ = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// データ取得
			if (reader.Read())
			{
				; // 郵便番号マスタは検索しない
			}
			else
			{
				// 終了処理
				disposeReader(reader);
				reader = null;
// ADD 2008.10.31 東都）高木 着店コード検索時に着店非表示チェックを追加 END
				// SQL文
				cmdQuery
					= "SELECT CM14.店所ＣＤ, CM10.店所名, CM14.都道府県ＣＤ || CM14.市区町村ＣＤ || CM14.大字通称ＣＤ \n"
					+ "  FROM ＣＭ１４郵便番号 CM14 \n"
					+ " INNER JOIN ＣＭ１０店所 CM10 \n"
					+ "    ON CM14.店所ＣＤ = CM10.店所ＣＤ \n"
					+ "   AND CM10.削除ＦＧ = '0' \n"
					+ " WHERE CM14.郵便番号 = '" + sYuubin + "' \n"
					+ "   AND LENGTH(TRIM(CM14.店所ＣＤ)) > 0 \n"
					+ "   AND CM14.削除ＦＧ = '0' \n";

				// SQL実行
				// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
				//reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

				cmdQuery
					= "SELECT CM14.店所ＣＤ, CM10.店所名, CM14.都道府県ＣＤ || CM14.市区町村ＣＤ || CM14.大字通称ＣＤ \n"
					+ "  FROM ＣＭ１４郵便番号 CM14 \n"
					+ " INNER JOIN ＣＭ１０店所 CM10 \n"
					+ "    ON CM14.店所ＣＤ = CM10.店所ＣＤ \n"
					+ "   AND CM10.削除ＦＧ = '0' \n"
					+ " WHERE CM14.郵便番号 = :p_YuubinNo \n"
					+ "   AND LENGTH(TRIM(CM14.店所ＣＤ)) > 0 \n"
					+ "   AND CM14.削除ＦＧ = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

				// データ取得
				if (reader.Read())
				{
					// 該当データあり

					// データ取得
					tenCD    = reader.GetString(0).Trim();		// 店所コード
					tenName  = reader.GetString(1).Trim();		// 店所名
					juusyoCD = reader.GetString(2).Trim();		// 住所コード

					// 戻り値をセット
					sRet[0] = " ";
					sRet[1] = tenCD;
					sRet[2] = tenName;
					sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
					// ↑↑ 荷受人マスタの住所コードを優先する

					// 終了処理
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 START
				else 
				{
					// ＣＭ１４に該当データなし

					// 戻り値をセット
					sRet[0] = "入力されたお届け先(郵便番号)では配達店が決められませんでした";
					sRet[1] = "0000";
					sRet[2] = " ";
					sRet[3] = " ";

					// 終了処理
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 END
// ADD 2008.10.31 東都）高木 着店コード検索時に着店非表示チェックを追加 START
			}
// ADD 2008.10.31 東都）高木 着店コード検索時に着店非表示チェックを追加 END
			// 終了処理
			disposeReader(reader);
			reader = null;

			///
			/// ＜第３段階＞
			/// 郵便住所マスタから着店コードを検索
			/// 
			// SQL文
			cmdQuery
				= "SELECT CM19.店所ＣＤ, CM10.店所名, CM19.住所ＣＤ, CM19.住所 \n"
				+ "  FROM ＣＭ１９郵便住所 CM19 \n"
				+ " INNER JOIN ＣＭ１０店所 CM10 \n"
				+ "    ON CM19.店所ＣＤ = CM10.店所ＣＤ \n"
				+ "   AND CM10.削除ＦＧ = '0' \n"
				+ " WHERE CM19.郵便番号 = '" + sYuubin + "' \n"
				+ "   AND CM19.削除ＦＧ = '0' \n"
// ADD 2009.01.16 kcl)森本 着店コードの検索方法を再変更 START
				+ " ORDER BY "
				+ "       LENGTH(TRIM(CM19.住所)) DESC \n"
// ADD 2009.01.16 kcl)森本 着店コードの検索方法を再変更 END
				;

			// SQL実行
			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery
				= "SELECT CM19.店所ＣＤ, CM10.店所名, CM19.住所ＣＤ, CM19.住所 \n"
				+ "  FROM ＣＭ１９郵便住所 CM19 \n"
				+ " INNER JOIN ＣＭ１０店所 CM10 \n"
				+ "    ON CM19.店所ＣＤ = CM10.店所ＣＤ \n"
				+ "   AND CM10.削除ＦＧ = '0' \n"
				+ " WHERE CM19.郵便番号 = :p_YuubinNo \n"
				+ "   AND CM19.削除ＦＧ = '0' \n"
				+ " ORDER BY "
				+ "       LENGTH(TRIM(CM19.住所)) DESC \n"
				;
			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// データ取得
// MOD 2008.12.24 kcl)森本 着店コードの検索方法を再変更 START
//			int dst;					// 類似度
//			int minDst = 999999;		// 最小の類似度（最も似てるやつ）
//			int len;					// 検索する住所の長さ
//			int maxLen = 0;				// 最小類似度の住所の最大長さ
//			while (reader.Read())
//			{
//				// 住所の取得
//				address = reader.GetString(3).Trim();
//				len = address.Length;
//
//				// 類似度の算出
//				dst = this.GetSED(sJuusyo, address);
//
//				if (dst < minDst) 
//				{
//					// 今までのより似てる
//
//					// データ取得
//					tenCD    = reader.GetString(0).Trim();		// 店所コード
//					tenName  = reader.GetString(1).Trim();		// 店所名
//					juusyoCD = reader.GetString(2).Trim();		// 住所コード
//					minDst   = dst;
//					maxLen   = len;
//				} 
//				else if (dst == minDst) 
//				{
//					// 今までのと同じくらい似てる
//
//					// 検索した住所の長さをチェック
//					if (len > maxLen) 
//					{
//						// 検索した住所が今までより長い
//
//						// データ取得
//						tenCD    = reader.GetString(0).Trim();	// 店所コード
//						tenName  = reader.GetString(1).Trim();	// 店所名
//						juusyoCD = reader.GetString(2).Trim();	// 住所コード
//						maxLen   = len;
//					}
//				}
//			}
//			if (tenCD.Length > 0) 
//			{
//				// 該当データあり
//
//				// 戻り値をセット
//				sRet[0] = " ";
//				sRet[1] = tenCD;
//				sRet[2] = tenName;
//				sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
//				// ↑↑ 荷受人マスタの住所コードを優先する
//
//				// 終了処理
//				disposeReader(reader);
//				reader = null;
//		
//				return sRet;
//			}
			while (reader.Read()) 
			{
				// 住所の取得
				address = reader.GetString(3).Trim();

// ADD 2009.02.20 kcl)森本 暫定対応 START
				if (sShimei == null) sShimei = " ";
// ADD 2009.02.20 kcl)森本 暫定対応 END

				// 住所・氏名のチェック
				if ((sJuusyo.IndexOf(address) >= 0) ||
					(sShimei.IndexOf(address) >= 0))
				{
					// データ取得
					tenCD    = reader.GetString(0).Trim();	// 店所コード
					tenName  = reader.GetString(1).Trim();	// 店所名
					juusyoCD = reader.GetString(2).Trim();	// 住所コード

					// 戻り値をセット
					sRet[0] = " ";
					sRet[1] = tenCD;
					sRet[2] = tenName;
					sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
					// ↑↑ 荷受人マスタの住所コードを優先する

					// 終了処理
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
			}
// MOD 2008.12.24 kcl)森本 着店コードの検索方法を再変更 END

			// 終了処理
			disposeReader(reader);
			reader = null;

			// 該当データ無
// MOD 2008.11.19 東都）高木 着店コードが空白でもエラーでなくする START
//			sRet[0] = "入力されたお届け先(郵便番号)では配達店が決められませんでした";
//			sRet[1] = "0000";
			sRet[0] = " ";
			sRet[1] = " ";
// MOD 2008.11.19 東都）高木 着店コードが空白でもエラーでなくする END
			sRet[2] = " ";
			sRet[3] = " ";
			
			return sRet;
		}

		/*********************************************************************
		 * 住所コード取得
		 * 　　ＣＭ１４郵便番号を使用して、郵便番号から住所コードを取得する。
		 * 引数：郵便番号
		 * 戻値：ステータス、住所ＣＤ
		 *
		 * Create : 2008.06.16 kcl)森本
		 * 　　　　　　新規作成
		 * Modify : 
		 *********************************************************************/
		private String[] Get_juusyoCode(string[] sUser, OracleConnection conn2, 
			string sYuubin)
		{
			string [] sRet = new string [2];	// 戻り値
			string cmdQuery;					// SQL文
			OracleDataReader reader;
			// ADD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// SQL文
			cmdQuery
				= "SELECT CM14.都道府県ＣＤ || CM14.市区町村ＣＤ || CM14.大字通称ＣＤ \n"
				+ "  FROM ＣＭ１４郵便番号 CM14 \n"
				+ " WHERE CM14.郵便番号 = '" + sYuubin + "' \n"
				+ "   AND CM14.削除ＦＧ = '0' \n";

			// SQL実行
			// MOD-S 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###バインド後（想定）###\n" + cmdQuery);	//修正前のUPDATE文をログ出力

			cmdQuery
				= "SELECT CM14.都道府県ＣＤ || CM14.市区町村ＣＤ || CM14.大字通称ＣＤ \n"
				+ "  FROM ＣＭ１４郵便番号 CM14 \n"
				+ " WHERE CM14.郵便番号 = :p_YuubinNo \n"
				+ "   AND CM14.削除ＦＧ = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)横山 Oracleサーバ負荷軽減対策（SQLにバインド変数を利用）

			// データ取得
			if (reader.Read())
			{
				// 該当データあり
				sRet[0] = " ";							// ステータス
				sRet[1] = reader.GetString(0).Trim();	// 住所コード
			} 
			else
			{
				// 該当データ無
				sRet[0] = "入力された郵便番号では住所コードが決められませんでした";
				sRet[1] = " ";
			}

			// 終了処理
			disposeReader(reader);
			reader = null;
			
			return sRet;
		}

		/*********************************************************************
		 * SED(Shortest Edit Distance)の取得
		 * 　　２つの文字列の類似度（最小エディット距離、SED）を取得します。
		 * 　　戻り値が小さいほどよく似ています。
		 * 引数：検索元の文字列、検索する文字列
		 * 戻値：２つの文字列のSED
		 *
		 * Create : 2008.06.12 kcl)森本
		 * 　　　　　　新規作成
		 * Modify : 
		 *********************************************************************/
		private int GetSED(string srcStr, string fndStr) 
		{
			int i, j;
			int srcLen = 0;
			int fndLen = 0;
			int minDst = 999999;

			// Unicode 文字列を文字毎に扱うための準備
			TextElementEnumerator srcTee = StringInfo.GetTextElementEnumerator(srcStr);
			TextElementEnumerator fndTee = StringInfo.GetTextElementEnumerator(fndStr);

			// 各文字列の文字数を計算
			while (srcTee.MoveNext()) 
				srcLen++;
			while (fndTee.MoveNext())
				fndLen++;
			srcTee.Reset();
			fndTee.Reset();

			// 文字列間の距離を算出するための配列の初期化
			int [,] C = new int[fndLen+1, srcLen+1];
			for (i=0; i<=fndLen; i++) 
				C[i, 0] = i;
			for (j=0; j<=srcLen; j++) 
				C[0, j] = 0;

			// 文字列間の距離を算出
			for (i=1; i<=fndLen; i++) 
			{
				fndTee.MoveNext();
				for (j=1; j<=srcLen; j++) 
				{
					srcTee.MoveNext();
					if (srcTee.Current.Equals(fndTee.Current)) 
						C[i, j] = C[i-1, j-1];
					else
						C[i, j] = 1 + Math.Min(Math.Min(C[i-1, j], C[i, j-1]), C[i-1, j-1]);
				}
				srcTee.Reset();
			}

			// 文字列間の最小距離を取得
			for (j=0; j<=srcLen; j++) 
				minDst = Math.Min(C[fndLen, j], minDst);

			return minDst;
		}

		/*********************************************************************
		 * 自動出荷登録用住所取得３
		 * 　　ＳＭ０２荷受人、ＣＭ１４郵便番号、ＣＭ１５着店非表示、ＣＭ１９郵便住所
		 *     の３マスタを使用して着店コードを決定する。
		 * 引数：会員コード、部門コード、荷受人コード、郵便番号、住所、氏名
		 * 戻値：ステータス、店所ＣＤ、店所名、住所ＣＤ
		 *
		 * Create : 2008.06.12 kcl)森本
		 * 　　　　　　Get_autoEntryPref を元に作成
		 * Modify : 2008.12.25 kcl)森本
		 *            引数に氏名を追加
		 *********************************************************************/
		[WebMethod]
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 START
//		public string [] Get_autoEntryPref3(string [] sUser, 
//			string sKaiinCode, string sBumonCode, string sNiukeCode, 
//			string sYuubin, string sJuusyo)
		public string [] Get_autoEntryPref3(string [] sUser, 
			string sKaiinCode, string sBumonCode, string sNiukeCode, 
			string sYuubin, string sJuusyo, string sShimei)
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 END
		{
			// ログ出力
			logWriter(sUser, INF, "住所取得３開始");

			OracleConnection conn2 = null;
			string [] sRet = new string [4];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// ＤＢ接続に失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			try {
				// 着店コードの取得
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 START
//				string [] sResult = this.Get_tyakuten3(sUser, conn2, sKaiinCode, sBumonCode, sNiukeCode, sYuubin, sJuusyo);
				string [] sResult = this.Get_tyakuten3(sUser, conn2, sKaiinCode, sBumonCode, sNiukeCode, sYuubin, sJuusyo, sShimei);
// MOD 2008.12.25 kcl)森本 着店コードの検索方法を再変更 END

				if (sResult[0] == " ")
				{
					// 取得成功
					sRet[1] = sResult[3];	// 住所ＣＤ
					sRet[2] = sResult[1];	// 店所ＣＤ
					sRet[3] = sResult[2];	// 店所名

					sRet[0] = "正常終了";
				}
				else
				{
					// 取得失敗
					sRet[0] = "該当データがありません";
				}

				// ログ出力
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle のエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// 終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2008.06.12 kcl)森本 着店コード検索方法の変更 END

// ADD 2015.07.14 bevas)松本 バーコード読取専用画面の追加 START
		/*********************************************************************
		 * グローバル集荷店件数取得
		 * 　　ＣＭ２０扱い店マスタを使用して、集荷店の件数を取得する。
		 * 引数：会員コード
		 * 戻値：ステータス、集荷店件数
		 *********************************************************************/
		[WebMethod]
		public string[] Get_GlobalCount(string[] sUser)
		{
			// ログ出力
			logWriter(sUser, INF, "グローバル集荷店件数取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]; // 戻り値
			string cmdQuery; // SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// ＤＢ接続に失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			try 
			{
				// SQL文
				cmdQuery
					= "SELECT COUNT(*) \n"
					+ "  FROM ＣＭ０５会員扱店 \n"
					+ " WHERE 会員ＣＤ = :p_KaiinCD \n"
					+ "   AND 削除ＦＧ = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sUser[0], ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// データ取得
				if (reader.Read())
				{
					// 該当データあり
					sRet[0] = "正常終了";						// ステータス
					sRet[1] = reader.GetDecimal(0).ToString();	// 集荷店件数
				} 
				else
				{
					// 該当データなし
					sRet[0] = "該当するデータは存在しません。";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle のエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// 終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * 修正可能出荷データ件数取得
		 *		ＳＴ０１出荷ジャーナルから、
		 *		出荷日翌日以降も修正可能な出荷データの件数を取得する。
		 * 引数 : 原票番号
		 * 戻値 : ステータス、出荷データ件数
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ModifiableSyukkaCount(string[] sUser, string sInvoiceNo)
		{
			// ログ出力
			logWriter(sUser, INF, "修正可能出荷データ件数取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]; // 戻り値
			string cmdQuery; // SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// ＤＢ接続に失敗
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			//送り状番号をテーブル格納形式に変換
			string sKey_InvoiceNo = string.Empty;
			sKey_InvoiceNo = "0000" + sInvoiceNo;

			try 
			{
				// SQL文
				cmdQuery
					= "SELECT COUNT(*) \n"
					+ "  FROM ＳＴ０１出荷ジャーナル \n"
					+ " WHERE 発店ＣＤ   = '047' \n"
					+ "   AND 送り状番号 = :p_InvoiceNo \n"
					+ "   AND 削除ＦＧ   = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_InvoiceNo", OracleDbType.Char, sKey_InvoiceNo, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// データ取得
				if (reader.Read())
				{
					// 該当データあり
					sRet[0] = "正常終了";						// ステータス
					sRet[1] = reader.GetDecimal(0).ToString();	// 出荷データ件数
				} 
				else
				{
					// 該当データなし
					sRet[0] = "該当するデータは存在しません。";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle のエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// 終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2015.07.14 bevas)松本 バーコード読取専用画面の追加 END
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 START
		/*********************************************************************
		 * ＣＭ１０存在チェック（支店止め用）
		 * 　　入力された郵便番号と住所３から、
		 * 　　ＣＭ１０店所の存在チェックを実施する。
		 * 引数：住所３（店所コードが格納）、郵便番号
		 * 戻値：ステータス、店所正式名
		 *********************************************************************/
		[WebMethod]
		public string[] CheckCM10_GeneralDelivery(string[] sUser, string sJusyo3, string sYubinNo)
		{
			logWriter(sUser, INF, "ＣＭ１０存在チェック（支店止め用）開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];	// 戻り値
			string cmdQuery;				// SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// 住所３に設定された店所コード（全角数字）を半角数字変換
			string sTyakutenCode = this.半角数字変換(sJusyo3);
			if(sTyakutenCode.Length != 3)
			{
				// 半角変換失敗
				sRet[0] = sTyakutenCode;
				sRet[1] = " ";
				return sRet;
			}

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				// SQL文
				cmdQuery
					= "SELECT 店所正式名 \n"
					+ "  FROM ＣＭ１０店所 \n"
					+ " WHERE 店所ＣＤ = :p_BranchCode \n"
					+ "   AND 郵便番号 = :p_YubinNo \n"
					+ "   AND 削除ＦＧ = '0' \n";
				if(sUser[0].Substring(0,1) != "J")
				{
					cmdQuery += "   AND 支店止めＦＧ１ = '1' \n"; // 支店止めＦＧ１（福山通運）

				}
				else
				{
					cmdQuery += "   AND 支店止めＦＧ２ = '1' \n"; // 支店止めＦＧ１（王子運送）
				}

				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_BranchCode", OracleDbType.Char, sTyakutenCode, ParameterDirection.Input); // 店所ＣＤ
				wk_opOraParam[1] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // 郵便番号

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// データ取得
				if (reader.Read())
				{
					// 該当データあり
					sRet[0] = "正常終了";								// ステータス
					sRet[1] = reader.GetString(0).ToString().Trim();	// 店所正式名
				} 
				else
				{
					// 該当データなし
					sRet[0] = "店所マスタ存在チェック：該当する店所データは存在しません。";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle のエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = " ";
			}
			catch (Exception ex)
			{
				// それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				sRet[1] = " ";
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// 終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * 着店取得（支店止め用）
		 * 　　ＣＭ１０店所マスタを使用して、着店を決定する。
		 * 引数：店所コード、郵便番号
		 * 戻値：ステータス、店所名
		 *********************************************************************/
		private string[] Get_tyakuten_GeneralDelivery(string[] sUser, OracleConnection conn2, string sTyakutenCode, string sYubinNo)
		{
			string[] sRet = new string[2];	// 戻り値
			string cmdQuery;				// SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// SQL文
			cmdQuery
				= "SELECT 店所名 \n"
				+ "  FROM ＣＭ１０店所 \n"
				+ " WHERE 店所ＣＤ = :p_BranchCode \n"
				+ "   AND 郵便番号 = :p_YubinNo \n"
				+ "   AND 支店止めＦＧ１ = '1' \n"
				+ "   AND 削除ＦＧ = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_BranchCode", OracleDbType.Char, sTyakutenCode, ParameterDirection.Input); // 店所ＣＤ
			wk_opOraParam[1] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // 郵便番号

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;

			// データ取得
			if (reader.Read())
			{
				// 該当データあり
				sRet[0] = " ";							// ステータス
				sRet[1] = reader.GetString(0).Trim();	// 店所名
			} 
			else
			{
				// 該当データなし
				sRet[0] = "支店止め着店決定：該当する店所データは存在しません。";
				sRet[1] = " ";
			}
			
			// 終了処理
			disposeReader(reader);
			reader = null;

			return sRet;
		}

		private string 半角数字変換(string sData)
		{
			// 桁数チェック
			if(sData.Length != 3)
			{
				return "店所コードが３桁ではありません。";
			}

			string wk = "";
			for(int i = 0; i < sData.Length; i++)
			{
				string sData_letter = sData.Substring(i, 1);
				switch(sData_letter)
				{
					case "０":
						sData_letter = sData_letter.Replace("０", "0");
						break;
					case "１":
						sData_letter = sData_letter.Replace("１", "1");
						break;
					case "２":
						sData_letter = sData_letter.Replace("２", "2");
						break;
					case "３":
						sData_letter = sData_letter.Replace("３", "3");
						break;
					case "４":
						sData_letter = sData_letter.Replace("４", "4");
						break;
					case "５":
						sData_letter = sData_letter.Replace("５", "5");
						break;
					case "６":
						sData_letter = sData_letter.Replace("６", "6");
						break;
					case "７":
						sData_letter = sData_letter.Replace("７", "7");
						break;
					case "８":
						sData_letter = sData_letter.Replace("８", "8");
						break;
					case "９":
						sData_letter = sData_letter.Replace("９", "9");
						break;
				}
				wk += sData_letter;
				sData_letter = "";
			}

			// 変換後の店所コードが不正であった場合
			if(wk.Length != 3)
			{
				return "変換後の店所コードが、半角数字３桁ではありません。不正な形式です。";
			}

			return wk;
		}
// MOD 2015.07.30 BEVAS) 松本 支店止め機能対応 END
// MOD 2015.12.15 BEVAS) 松本 輸送禁止エリア機能対応 START
		/*********************************************************************
		 * 配達不能エリアチェック
		 * 　　入力された郵便番号から、
		 * 　　ＣＭ２１配達不能の存在チェックを実施する。
		 * 引数：郵便番号
		 * 戻値：ステータス、検索文字、メッセージ、表示開始日、表示終了日
		 *********************************************************************/
		[WebMethod]
		public ArrayList Check_NonDeliveryArea(string[] sUser, string sYubinNo)
		{
			logWriter(sUser, INF, "配達不能エリアチェック開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];  //主にステータスを格納
			ArrayList alRet = new ArrayList();  //戻り値
			string cmdQuery;  // SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				// SQL文
				cmdQuery
					= "SELECT 検索文字, メッセージ, 表示開始日, 表示終了日 \n"
					+ "  FROM ＣＭ２１配達不能 \n"
					+ " WHERE 郵便番号 = :p_YubinNo \n"
					+ "   AND 削除ＦＧ = '0' \n"
					+ " ORDER BY 検索文字 DESC, メッセージ DESC \n";
				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // 郵便番号

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// データ取得
				while(reader.Read())
				{
					string[] sData = new string[4];
					sData[0] = reader.GetString(0).Trim(); // 検索文字
					sData[1] = reader.GetString(1).Trim(); // メッセージ
					sData[2] = reader.GetString(2).Trim(); // 表示開始日
					sData[3] = reader.GetString(3).Trim(); // 表示終了日

					alRet.Add(sData); //リストに格納
				}

				disposeReader(reader);
				reader = null;

				if(alRet.Count == 0)
				{
					//該当データなし
					sRet[0] = "該当データなし";
					alRet.Add(sRet);
				}
				else
				{
					//該当データあり
					sRet[0] = "該当データあり";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				// Oracle のエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				// それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// 終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return alRet;
		}
// MOD 2015.12.15 BEVAS) 松本 輸送禁止エリア機能対応 END
// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 START
		/*********************************************************************
		 * 発店、集約店取得（社内伝票用）
		 * 　　ＣＭ０５会員扱店Ｆ、ＣＭ１０店所、ＣＭ１１集約店を使用して
		 * 　　発店、集約店を決定する
		 * 引数：会員コード
		 * 戻値：ステータス、発店ＣＤ、発店名、集約店ＣＤ
		 * 
		 *********************************************************************/
		private string[] Get_hatuten_HouseSlip(string[] sUser, OracleConnection conn2, string sKaiinCode)
		{
			string[] sRet = new string[4];	// 戻り値
			string cmdQuery;				// SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// SQL文
			cmdQuery
				= " SELECT \n"
				+ "  CM05F.店所ＣＤ \n"
				+ " ,NVL(CM10.店所名, ' ') \n"
// MOD 2016.06.23 bevas) 松本 社内伝票対応のバグ修正 START
				//集約店ＣＤの参照方法を変更
//				+ " ,NVL(CM11.集約店ＣＤ, ' ') \n"
				+ " ,NVL(CM10.集約店ＣＤ, '0000') \n"
// MOD 2016.06.23 bevas) 松本 社内伝票対応のバグ修正 END
				+ " FROM ＣＭ０５会員扱店Ｆ CM05F \n"
				+ " LEFT JOIN ＣＭ１０店所 CM10 \n"
				+ "    ON CM10.店所ＣＤ = CM05F.店所ＣＤ "
				+ "   AND CM10.削除ＦＧ = '0' \n"
// MOD 2016.06.23 bevas) 松本 社内伝票対応のバグ修正 START
				//親店の場合、集約店マスタのレコードに削除ＦＧが立っていることの考慮が漏れていた為、対応
//				+ " LEFT JOIN ＣＭ１１集約店 CM11 \n"
//				+ "    ON CM11.集荷店ＣＤ = CM10.店所ＣＤ "
//				+ "   AND CM11.削除ＦＧ = '0' \n"
// MOD 2016.06.23 bevas) 松本 社内伝票対応のバグ修正 END
				+ " WHERE CM05F.会員ＣＤ = :p_MemberCode \n"
				+ "   AND CM05F.削除ＦＧ = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_MemberCode", OracleDbType.Char, sKaiinCode, ParameterDirection.Input); // 会員ＣＤ

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;

			// データ取得
			if(reader.Read())
			{
				// 該当データあり
				sRet[0] = " ";							// ステータス
				sRet[1] = reader.GetString(0).Trim();	// 発店コード
				sRet[2] = reader.GetString(1).Trim();	// 発店名
				sRet[3] = reader.GetString(2).Trim();	// 集約店コード
			} 
			else
			{
				// 該当データなし
				sRet[0] = "社内伝発店等決定：該当する会員扱店データは存在しません。"; // ステータス
				sRet[1] = "0000";                                                     // 発店コード
				sRet[2] = " ";                                                        // 発店名
				sRet[3] = "0000";                                                     // 集約店コード
			}
			
			// 終了処理
			disposeReader(reader);
			reader = null;

			return sRet;
		}

		/*********************************************************************
		 * ＣＭ０５Ｆ存在チェック（社内伝票用）
		 * 　　社内伝ログインユーザーに対して、出荷登録／更新前に
		 * 　　ＣＭ０５会員扱店Ｆの存在チェックを実施する。
		 * 引数：ログインユーザーの会員コード
		 * 戻値：ステータス、店所コード
		 * 
		 *********************************************************************/
		[WebMethod]
		public string[] CheckCM22_HouseSlip(string[] sUser, string sKaiinCD)
		{
			logWriter(sUser, INF, "ＣＭ０５Ｆ存在チェック（社内伝票用）開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];	//戻り値
			string cmdQuery;				//SQL文
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			//ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				//SQL文
				cmdQuery
					= "SELECT 店所ＣＤ \n"
					+ "  FROM ＣＭ０５会員扱店Ｆ \n"
					+ " WHERE 会員ＣＤ = :p_MemberCode \n"
					+ "   AND 削除ＦＧ = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_MemberCode", OracleDbType.Char, sKaiinCD, ParameterDirection.Input); //会員ＣＤ

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// データ取得
				if(reader.Read())
				{
					//該当データあり
					sRet[0] = "正常終了";                 //ステータス
					sRet[1] = reader.GetString(0).Trim(); //店所ＣＤ
				} 
				else
				{
					//該当データなし
					sRet[0] = "該当データがありません";   //ステータス
					sRet[1] = " ";                        //店所ＣＤ(空白)
				}
			}
			catch(OracleException ex)
			{
				//Oracleのエラー
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = " ";
			}
			catch(Exception ex)
			{
				//それ以外のエラー
				sRet[0] = "サーバエラー：" + ex.Message;
				sRet[1] = " ";
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				//終了処理
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * 発店取得（ＣＳＶ自動監視機能使用時：社内伝票会員用）
		 * 引数：会員ＣＤ
		 * 戻値：ステータス、店所ＣＤ、店所名
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_hatuten2_HouseSlip(string[] sUser, string sKcode)
		{
			logWriter(sUser, INF, "発店（社内伝票会員用）取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];

			//ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery =
					  "SELECT CM05F.店所ＣＤ, NVL(CM10.店所名, ' ') \n"
					+ " FROM ＣＭ０５会員扱店Ｆ CM05F, \n"
					+      " ＣＭ１０店所 CM10 \n"
					+ " WHERE CM05F.会員ＣＤ = '" + sKcode + "' \n"
					+ " AND CM05F.削除ＦＧ = '0' \n"
					+ " AND CM05F.店所ＣＤ = CM10.店所ＣＤ \n"
					+ " AND CM10.削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
				disposeReader(reader);
				reader = null;
			}
			catch(OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch(Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * 集約店取得（ＣＳＶ自動監視機能使用時：社内伝票会員用）
		 * 引数：会員ＣＤ
		 * 戻値：ステータス、集約店ＣＤ
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syuuyakuten2_HouseSlip(string[] sUser, string sKcode)
		{
			logWriter(sUser, INF, "集約店（社内伝票会員用）取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];

			//ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT CM10.集約店ＣＤ \n"
					+ " FROM ＣＭ０５会員扱店Ｆ CM05F, ＣＭ１０店所 CM10 \n"
					+ " WHERE CM05F.会員ＣＤ = '" + sKcode + "' \n"
					+ "   AND CM05F.削除ＦＧ = '0' \n"
					+    "AND CM05F.店所ＣＤ = CM10.店所ＣＤ \n"
					+ "   AND CM10.削除ＦＧ = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[0] = "正常終了";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "該当データがありません";
				}
				disposeReader(reader);
				reader = null;
			}
			catch(OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch(Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2016.04.08 bevas) 松本 社内伝票機能追加対応 END
	}
}
