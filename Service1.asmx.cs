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
	// �C������
	//--------------------------------------------------------------------------
	// ADD 2007.04.20 ���s�j���� �b�r�u�G���g���i�����o�דo�^�j�̍�����
	//	Check_autoEntry1	�R�̊֐����P��
	//	Get_Stodokesaki2	�֐��̃V���v����
	//	Get_autoEntryPref2	�֐��̃V���v����
	//	Get_autoEntryClaim2	�֐��̃V���v����
	//--------------------------------------------------------------------------
	//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή�
	//	Get_JurnalNo �̔ԊǗ��̊֐���
	//	�����o�דo�^ �d���b�c�̐ݒ�̍�����
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j��
	//	disposeReader(reader);
	//	reader = null;
	// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	// ADD 2007.07.06 ���s�j���� �ꗗ���ڂɔ��X�b�c��\�� 
	// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� 
	//--------------------------------------------------------------------------
	// MOD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX
	//  Ins_syukka
	//  Upd_syukka
	// ADD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX�ɔ����A���\�b�h�ǉ�
	//  Get_tyakuten3
	//  GetSED
	//  Get_autoEntryPref3
	// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 
	// MOD 2008.06.17 ���s�j���� �^�����O�̏ꍇ[��]�\�� 
	// ADD 2008.07.09 ���s�j���� �����s�������O���� 
	// MOD 2008.10.16 kcl)�X�{ ���X�R�[�h���� Empty �ɂȂ�Ȃ��悤�ɂ���
	// ADD 2008.10.29 ���s�j���� ���������ǉ� 
	// ADD 2008.10.31 ���s�j���� ���X�R�[�h�������ɒ��X��\���`�F�b�N��ǉ� 
	// MOD 2008.11.19 ���s�j���� ���X�R�[�h���󔒂ł��G���[�łȂ����� 
	// MOD 2008.12.24 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX
	//--------------------------------------------------------------------------
	// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V 
	// MOD 2009.04.02 ���s�j���� �ғ����Ή� 
	// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� 
	// MOD 2009.09.11 ���s�j���� �o�׏Ɖ�ŏo�׍ςe�f,���M�ςe�f�Ȃǂ�ǉ� 
	// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���j
	// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V 
	// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� 
	//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� 
	// MOD 2010.07.30 ���s�j���� �����o�׎��̉ב��l���擾�̒��� 
	// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� 
	// MOD 2010.10.13 ���s�j���� [�i���L���S]�ȂǍ��ڒǉ� 
	// MOD 2010.10.27 ���s�j���� �폜�����Ȃǂ̒ǉ� 
	//�ۗ� MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V 
	// MOD 2010.11.10 ���s�j���� �X�V�ҁA�X�V�o�f�̍��ڂ̏C�� 
	// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� 
	//--------------------------------------------------------------------------
	// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� 
	// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� 
	// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� 
	// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX 
	// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� 
	// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� 
	// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
	//--------------------------------------------------------------------------
	// MOD 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
	// MOD 2012.10.01 COA)���R Oracle�T�[�o���׌y���΍�
	//                         �iORA-01461�ɂ��AST01�ւ�INSERT�����̃R�[�h�ɖ߂��j
	//--------------------------------------------------------------------------
	// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ�
	// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ�
	//--------------------------------------------------------------------------
	// ADD 2015.07.12 bevas)���{ �o�[�R�[�h�ǎ��p��ʂ̒ǉ�
	// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή�
	// MOD 2015.12.15 BEVAS) ���{ �A���֎~�G���A�@�\�Ή�
	//--------------------------------------------------------------------------
	// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�擾���ڒǉ��i�d�ʁA�ː��j
	// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή�
	// MOD 2016.06.23 bevas) ���{ �Г��`�[�Ή��̃o�O�C��
	//                            �i����̈��X���e�X�̂Ƃ��A�W��X���󔒂ɂȂ�j
	// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ�
	//                            �i�o�׍X�V�E�폜�������́A�����O�S���u0�v�ɂ���j
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
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * �o�׈ꗗ�擾
		 * �����F����b�c�A����b�c�A�׎�l�b�c�A�ב��l�b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I�����A���
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�o�ד��A�Z���P�A���O�P�A�j...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_1 
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//			= "SELECT NVL(TO_CHAR(COUNT(*) ,'999,999'),'0'), \n"
//			+       " NVL(TO_CHAR(SUM(��),'999,999'),'0'), \n"
//			+       " NVL(TO_CHAR(SUM(�d��),'999,999,999'),'0') \n"
//			+  " FROM \"�r�s�O�P�o�׃W���[�i��\" S, �`�l�O�R��� J \n";
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
//			+       " NVL(COUNT(*),0), \n"
			+       " NVL(COUNT(S.ROWID),0), \n"
			+       " NVL(SUM(S.��),0), \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
////			+       " NVL(SUM(S.�d��),0), \n"
////// ADD 2005.05.16 ���s�j�����J �ː��ǉ� START
////			+       " NVL(SUM(S.�ː�),0) \n"
////// ADD 2005.05.16 ���s�j�����J �ː��ǉ� END
//			+       " NVL(SUM(DECODE(S.�^���d��,'     ',0,S.�^���d��)),0), \n"
//			+       " NVL(SUM(DECODE(S.�^���ː�,'     ',0,S.�^���ː�)),0), \n"
//			+       " 1 \n"
//// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
			+       " NVL(SUM(S.�d��),0) \n"
			+       ", NVL(SUM(S.�ː�),0) \n"
			+       ", NVL(SUM(DECODE(S.�^���d��,'     ',0,S.�^���d��)),0) \n"
			+       ", NVL(SUM(DECODE(S.�^���ː�,'     ',0,S.�^���ː�)),0) \n"
			+       ", NVL(MAX(CM01.�ۗ�����e�f),'0') \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
			+  " FROM \"�r�s�O�P�o�׃W���[�i��\" S, �`�l�O�R��� J \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END

		private static string GET_SYUKKA_SELECT_2 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " SUBSTR(S.�o�ד�,5,2) || '/' || SUBSTR(S.�o�ד�,7,2), S.�Z���P, S.���O�P, \n"
			+       " TO_CHAR(S.��), S.�d��, S.�A���w���P, \n"
			+       " S.�i���L���P, S.�����ԍ�, DECODE(S.�����敪,1,'����',2,'����',S.�����敪), \n"
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
//			+       " DECODE(S.�w���,0,' ',(SUBSTR(S.�w���,5,2) || '/' || SUBSTR(S.�w���,7,2))), \n"
			+       " DECODE(S.�w���,0,' ',(SUBSTR(S.�w���,5,2) || '/' || SUBSTR(S.�w���,7,2) || DECODE(S.�w����敪,'0','�K��','1','�w��',''))), \n"
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
//			+       " DECODE(���,1,'�o�^��',2,'���s��',3,'�o�׍�',4,'�^�s��',5,'�z��',6,'�s��',7,'�ԕi',' '), \n"
//			+       " DECODE(���,'01','�����s','02','���s��','03','�o�׍�','04','�W��', \n"
//			+                   " '05','�^�s��','06','����'  ,'07','���o'  ,'08','�z��', \n"
//			+                   " '09',DECODE(�ڍ׏��,'01','�s��','02','�ԕi',�ڍ׏��),���), \n"

//			+       " DECODE(���,'09',NVL(J.��ԏڍז�, ' '), NVL(J.��Ԗ�, ' ')), \n"
			+       " DECODE(S.�ڍ׏��,'  ', NVL(J.��Ԗ�, S.���),NVL(J.��ԏڍז�, S.�ڍ׏��)), \n"
			+       " SUBSTR(S.�o�^��,5,2) || '/' || SUBSTR(S.�o�^��,7,2), \n"
			+       " S.���q�l�o�הԍ�, TO_CHAR(S.\"�W���[�i���m�n\"), S.�o�^��, \n"
			+       " SUBSTR(S.�o�ד�,1,4) || '/' || SUBSTR(S.�o�ד�,5,2) || '/' || SUBSTR(S.�o�ד�,7,2), \n"
// ADD 2005.05.11 ���s�j�����J �o�^�Ғǉ� START
			+       " S.�o�^��, \n"
// ADD 2005.05.11 ���s�j�����J �o�^�Ғǉ� END
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� START
			+       " S.�ː� \n"
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			+       ", S.�^���ː�, S.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" S, �`�l�O�R��� J \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;

		private static string GET_SYUKKA_SELECT_2_SORT
//			= " ORDER BY �o�ד�,�Z���P,���O�P,�����ԍ�,�o�^��,\"�W���[�i���m�n\" ";
			= " ORDER BY �o�^��,\"�W���[�i���m�n\" ";

		private static string GET_SYUKKA_SELECT_2_SORT2
//			= " ORDER BY �o�ד�,�Z���P,���O�P,�����ԍ�,�o�^��,\"�W���[�i���m�n\" ";
			= " ORDER BY �o�ד�,�o�^��,\"�W���[�i���m�n\" ";

		[WebMethod]
		public String[] Get_syukka(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׈ꗗ�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			string s�o�^���� = "0";
			string s�����v = "0";
// DEL 2005.06.08 ���s�j���� ���g�p�̈׍폜 START
//			string s�d�ʍ��v = "0";
// DEL 2005.06.08 ���s�j���� ���g�p�̈׍폜 END
			int    i�o�^���� = 0;
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� START
			decimal d�d�ʍ��v = 0;
			decimal d�ː����v = 0;
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� END
// ADD 2005.05.18 ���s�j�����J ����� START
			string s�����    = "";
// ADD 2005.05.18 ���s�j�����J ����� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː��d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
			decimal d�ː��d�ʍ��v = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			StringBuilder sbRet = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE S.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND S.����b�c = '" + sBCode + "' \n");

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND S.�׎�l�b�c = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND S.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND S.�׎�l�b�c = '"+ sTCode + "' \n");
					sbQuery.Append(" AND S.�ב��l�b�c = '"+ sICode + "' \n");
				}
// ADD 2005.06.01 ���s�j�����J �`���C�X�͓��t�͈͂Ȃ� START
//				if(iSyuka == 0)
//					sbQuery.Append(" AND S.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
//				else
//					sbQuery.Append(" AND S.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				if(sSday != "0")
				{
					if(iSyuka == 0)
						sbQuery.Append(" AND S.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
					else
						sbQuery.Append(" AND S.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				}
// ADD 2005.06.01 ���s�j�����J �`���C�X�͓��t�͈͂Ȃ� END
				
				if(sJyoutai != "00")
				{
					if(sJyoutai == "aa")
						sbQuery.Append(" AND S.��� <> '01' \n");
					else
						sbQuery.Append(" AND S.��� = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND S.�폜�e�f = '0' \n");
				sbQuery.Append(" AND S.���     = J.��Ԃb�c(+) \n");
				sbQuery.Append(" AND S.�ڍ׏�� = J.��ԏڍׂb�c(+) \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(" AND S.����b�c     = CM01.����b�c(+) \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				sbQuery2.Append(GET_SYUKKA_SELECT_1);
				sbQuery2.Append(sbQuery);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery2);

				if(reader.Read())
				{
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//					s�o�^����   = reader.GetString(0);
//					s�����v   = reader.GetString(1);
//					s�d�ʍ��v   = reader.GetString(2);
					s�o�^����   = reader.GetDecimal(0).ToString("#,##0").Trim();
					s�����v   = reader.GetDecimal(1).ToString("#,##0").Trim();
//					s�d�ʍ��v   = reader.GetDecimal(2).ToString("#,##0").Trim();
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� START
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					if(reader.GetString(6) == "1"){
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						d�d�ʍ��v   = reader.GetDecimal(2);
						d�ː����v   = reader.GetDecimal(3);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}else{
						d�d�ʍ��v   = reader.GetDecimal(4);
						d�ː����v   = reader.GetDecimal(5);
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// ADD 2005.05.16 ���s�j�����J �ː��ǉ� END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				sRet[1] = s�o�^����;
				sRet[2] = s�����v;
// MOD 2005.05.16 ���s�j�����J �ː��ǉ� START
//				sRet[3] = s�d�ʍ��v;
				d�d�ʍ��v = d�d�ʍ��v + d�ː����v * 8;
				sRet[3] = d�d�ʍ��v.ToString("#,##0").Trim();
////				if(s�d�ʍ��v == "0")
////				{
////					d�ː����v = d�ː����v * 8;
////					sRet[3] = d�ː����v.ToString("#,##0").Trim();
////				}
////				else
////				{
////					sRet[3] = s�d�ʍ��v;
////				}
// MOD 2005.05.16 ���s�j�����J �ː��ǉ� END

// MOD 2006.03.09 ���s�j���� �o�^�������P�O�O�O���𒴂���ƃG���[���������� START
//				i�o�^���� = int.Parse(s�o�^����);
				i�o�^���� = int.Parse(s�o�^����.Replace(",",""));
// MOD 2006.03.09 ���s�j���� �o�^�������P�O�O�O���𒴂���ƃG���[���������� END

				if(i�o�^���� == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
				else
				{
					sRet = new string[i�o�^���� + 4];
					sRet[0] = "����I��";
					sRet[1] = s�o�^����;
					sRet[2] = s�����v;
// MOD 2005.05.16 ���s�j�����J �ː��ǉ� START
//					sRet[3] = s�d�ʍ��v;
					sRet[3] = d�d�ʍ��v.ToString("#,##0").Trim();
////					if(s�d�ʍ��v == "0")
////						sRet[3] = d�ː����v.ToString("#,##0").Trim();
////					else
////						sRet[3] = s�d�ʍ��v;
// MOD 2005.05.16 ���s�j�����J �ː��ǉ� END

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

						sbRet.Append(sSepa + reader.GetString(0));			// �o�ד�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//						sbRet.Append(sSepa + reader.GetString(1).Trim());	// �Z���P
//						sbRet.Append(sCRLF + reader.GetString(2).Trim());	// ���O�P
						sbRet.Append(sSepa + reader.GetString(1).TrimEnd()); // �Z���P
						sbRet.Append(sCRLF + reader.GetString(2).TrimEnd()); // ���O�P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
						sbRet.Append(sSepa + reader.GetString(3));			// ��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//// MOD 2005.05.16 ���s�j�����J �ː��ǉ� START
////						sbRet.Append(sSepa + reader.GetString(4));			// �d��
//						d�ː����v = reader.GetDecimal(17);
//						d�ː����v = d�ː����v * 8;
//						if(d�ː����v == 0)
//							sbRet.Append(sSepa + reader.GetDecimal(4).ToString("#,##0").Trim()); // �d��
//						else
//							sbRet.Append(sSepa + d�ː����v.ToString("#,##0").Trim());		// �ː�
//// MOD 2005.05.16 ���s�j�����J �ː��ǉ� END
						s�^���ː� = reader.GetString(18).TrimEnd();
						s�^���d�� = reader.GetString(19).TrimEnd();
						d�ː��d�� = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
						s�d�ʓ��͐��� = reader.GetString(20).TrimEnd();
						if(s�d�ʓ��͐��� == "1" 
						&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
						){
							d�ː��d�� = reader.GetDecimal(17) * 8;	// �ː�
							d�ː��d�� += reader.GetDecimal(4);		// �d��
							sbRet.Append(sSepa + d�ː��d��.ToString("#,##0").Trim());
						}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
							if(s�^���ː�.Length > 0){
								try{
									d�ː��d�� += (Decimal.Parse(s�^���ː�) * 8);
								}catch(Exception){}
							}
							if(s�^���d��.Length > 0){
								try{
									d�ː��d�� += Decimal.Parse(s�^���d��);
								}catch(Exception){}
							}
							if(d�ː��d�� == 0){
								sbRet.Append(sSepa + " ");
							}else{
								sbRet.Append(sSepa + d�ː��d��.ToString("#,##0").Trim());
							}
//							// ���q�l���͒l
//							d�ː����v = reader.GetDecimal(17) * 8;
//							d�ː����v += reader.GetDecimal(4);
//							if(d�ː����v == 0){
//								;
//							}else{
//								sbRet.Append(sCRLF + "("
//											+ d�ː��d��.ToString("#,##0").Trim()
//											+ ")");
//							}
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
						}
						d�ː��d�ʍ��v += d�ː��d��;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());// �A���w���P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());	// �i���L���P
						sbRet.Append(sCRLF + reader.GetString(6).TrimEnd()); // �i���L���P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
// ADD 2005.05.18 ���s�j�����J ����� START
//						sbRet.Append(sSepa + reader.GetString(7).Trim());	// �����ԍ�
						s����� = reader.GetString(7).Trim();               // �����ԍ�
						if(s�����.Length == 0)
							sbRet.Append(sSepa + s�����);
						else
							sbRet.Append(sSepa + s�����.Remove(0,4));
// ADD 2005.05.18 ���s�j�����J ����� END
						sbRet.Append(sCRLF + reader.GetString(8));			// �����敪
						sbRet.Append(sSepa + reader.GetString(9));			// �w���
// MOD 2005.05.11 ���s�j���� ��ԕ\���̃o�O�C�� START
//						sbRet.Append(sSepa + reader.GetString(10));			// ���
						sbRet.Append(sSepa + reader.GetString(10).Trim());	// ���
// MOD 2005.05.11 ���s�j���� ��ԕ\���̃o�O�C�� END
						sbRet.Append(sSepa + reader.GetString(11));			// �o�^��
						sbRet.Append(sSepa + reader.GetString(12).Trim());	// ���q�l�o�הԍ�
						sbRet.Append(sSepa + reader.GetString(13));			// �W���[�i���m�n
						sbRet.Append(sSepa + reader.GetString(14));			// �o�^��
						sbRet.Append(sSepa + reader.GetString(15));			// �o�ד�
// ADD 2005.05.11 ���s�j�����J �o�^�Ғǉ� START
						sbRet.Append(sSepa + reader.GetString(16));			// �o�^��
// ADD 2005.05.11 ���s�j�����J �o�^�Ғǉ� END
						sbRet.Append(sSepa);
						sRet[iCnt] = sbRet.ToString();
						iCnt++;
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					sRet[3] = d�ː��d�ʍ��v.ToString("#,##0").Trim();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				sbQuery  = null;
				sbQuery2 = null;
				sbRet    = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �o�׃f�[�^�擾
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n
		 * �ߒl�F�X�e�[�^�X�A�o�ד��A���q�l�o�הԍ��A�׎�l�b�c�A�d�b�ԍ�...
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_Ssyukka(string[] sUser, string sKCode,string sBCode,string sDay, int iJNo)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׏��擾�J�n");

// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� START
// MOD 2005.05.13 ���s�j�����J �ב��l�d�ʒǉ� START
//			string[] sRet = new string[39];
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� START
//			string[] sRet = new string[40];
//			string[] sRet = new string[42];
			OracleConnection conn2 = null;
// MOD 2009.04.02 ���s�j���� �ғ����Ή� START
//			string[] sRet = new string[46];
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� START
//			string[] sRet = new string[47];
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� START
//			string[] sRet = new string[49];
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� START
//			string[] sRet = new string[51];
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//			string[] sRet = new string[53];
			string[] sRet = new string[56];
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� END
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� END
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� END
// MOD 2009.04.02 ���s�j���� �ғ����Ή� END
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� END
// MOD 2005.05.13 ���s�j�����J �ב��l�d�ʒǉ� END
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� END
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			try
			{
				string cmdQuery = "SELECT /*+ INDEX(S ST01PKEY) INDEX(N SM01PKEY) */ \n"
					+ " substr(J.�o�ד�,1,4) || '/' || substr(J.�o�ד�,5,2) || '/' || substr(J.�o�ד�,7,2), \n"
					+ "J.���q�l�o�הԍ�,J.�׎�l�b�c,J.�d�b�ԍ��P,J.�d�b�ԍ��Q,J.�d�b�ԍ��R, \n"
					+ "J.�Z���P,J.�Z���Q,J.�Z���R,J.���O�P,J.���O�Q,SUBSTR(J.�X�֔ԍ�,1,3),SUBSTR(J.�X�֔ԍ�,4,4), \n"
					+ "J.�ב��l�b�c,J.���ۖ�,TO_CHAR(J.��),TO_CHAR(NVL(J.�d��,'0')), \n"
					+ "DECODE(J.�w���,0,'0',substr(J.�w���,1,4) || '/' || substr(J.�w���,5,2) || '/' || substr(J.�w���,7,2)), \n"
					+ "J.�A���w���P,J.�A���w���Q,J.�i���L���P,J.�i���L���Q,J.�i���L���R, \n"
					+ "TO_CHAR(J.�ی����z),TO_CHAR(J.�X�V����), \n"
					+ "NVL(N.�d�b�ԍ��P,' '),NVL(N.�d�b�ԍ��Q,' '),NVL(N.�d�b�ԍ��R,' '), \n"
					+ "NVL(N.�Z���P,' '),NVL(N.�Z���Q,' '),NVL(N.���O�P,' '),NVL(N.���O�Q,' '), \n"
					+ "NVL(SUBSTR(N.�X�֔ԍ�,1,3),' '),NVL(SUBSTR(N.�X�֔ԍ�,4,4),' '), \n"
// MOD 2005.05.13 ���s�j�����J �ב��l�d�ʒǉ� START
//					+ "NVL(N.���Ӑ�b�c,' '),NVL(N.���Ӑ敔�ۂb�c,' '),J.�ב��l������ \n"
					+ "NVL(N.���Ӑ�b�c,' '),NVL(N.���Ӑ敔�ۂb�c,' '),J.�ב��l������,TO_CHAR(NVL(N.�d��,'0')), \n"
// MOD 2005.05.13 ���s�j�����J �ב��l�d�ʒǉ� END
// ADD 2005.05.17 ���s�j�����J �ː��ǉ� START
					+ "TO_CHAR(NVL(J.�ː�,'0')),TO_CHAR(NVL(N.�ː�,'0')) \n"
// ADD 2005.05.17 ���s�j�����J �ː��ǉ� END
// ADD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� START
					+ ",J.�A���w���b�c�P,J.�A���w���b�c�Q,J.�����ԍ� \n"
// ADD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� END
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
					+ ",J.�w����敪 \n"
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
// MOD 2009.04.02 ���s�j���� �ғ����Ή� START
					+ ",J.�o�^�o�f \n"
// MOD 2009.04.02 ���s�j���� �ғ����Ή� END
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� START
					+ ",J.���Ӑ�b�c, J.���ۂb�c \n"
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� END
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� START
					+ ", J.���, J.�o�׍ςe�f \n"
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� END
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� START
					+ ", J.����󔭍s�ςe�f, J.���M�ςe�f \n"
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					+ ", J.�i���L���S, J.�i���L���T, J.�i���L���U \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J,�r�l�O�P�ב��l N \n"
					+ " WHERE J.����b�c   = '" + sKCode + "' \n"
					+ "   AND J.����b�c   = '" + sBCode + "' \n"
					+ "   AND J.�o�^��     = '" + sDay + "' \n"
					+ "   AND J.\"�W���[�i���m�n\" = " + iJNo + " \n"
					+ "   AND J.�폜�e�f = '0' \n"
					+ "   AND J.�ב��l�b�c     = N.�ב��l�b�c(+) \n"
					+ "   AND '" + sKCode + "' = N.����b�c(+) \n"
					+ "   AND '" + sBCode + "' = N.����b�c(+) \n"
					+ "   AND '0' = N.�폜�e�f(+) ";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				bool bRead = reader.Read();
				if(bRead == true)
				{
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� START
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� START
//					for(int iCnt = 1; iCnt < 39; iCnt++)
//					for(int iCnt = 1; iCnt < 41; iCnt++)
//					for(int iCnt = 1; iCnt < 44; iCnt++)
					for(int iCnt = 1; iCnt < 45; iCnt++)
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� END
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� END
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
					{
						sRet[iCnt] = reader.GetString(iCnt - 1).TrimEnd();
					}
					sRet[0] = "����I��";
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� START
//					sRet[41] = "U";
//					sRet[44] = "U";
					sRet[45] = "U";
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� END
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
// MOD 2009.04.02 ���s�j���� �ғ����Ή� START
					sRet[46] = reader.GetString(44).TrimEnd();
// MOD 2009.04.02 ���s�j���� �ғ����Ή� END
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� START
					sRet[47] = reader.GetString(45).TrimEnd(); //J.���Ӑ�b�c
					sRet[48] = reader.GetString(46).TrimEnd(); //J.���ۂb�c
// MOD 2010.08.31 ���s�j���� ���s�̐�������̕\�� END
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� START
					sRet[49] = reader.GetString(47); // ���
					sRet[50] = reader.GetString(48); // �o�׍ςe�f
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� END
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� START
					sRet[51] = reader.GetString(49); // ����󔭍s�ςe�f
					sRet[52] = reader.GetString(50); // ���M�ςe�f
// MOD 2011.03.11 ���s�j���� �f�o���M�ρi�o�׍ρj�f�[�^�̏C�������̋��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					sRet[53] = reader.GetString(51).TrimEnd(); // �i���L���S
					sRet[54] = reader.GetString(52).TrimEnd(); // �i���L���T
					sRet[55] = reader.GetString(53).TrimEnd(); // �i���L���U
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�o�׏��擾["+sBCode+"]["+sDay+"]["+iJNo
									+"]:["+reader.GetString(42).TrimEnd()+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� START
//					sRet[41] = "I";
//					sRet[43] = "I";
					sRet[45] = "I";
// MOD 2005.06.01 ���s�j�ɉ� �A���w���R�[�h�ǉ� END
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �o�׃f�[�^�o�^
		 * �����F����b�c�A����b�c�A�o�ד�...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_syukka(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�דo�^�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			decimal d����;
			string s����v = " ";
			string s�o�^��;
			int i�Ǘ��m�n;
			string s���t;

			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			int					iUpdRow			= 0;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			try
			{
				//�o�ד��`�F�b�N
				string[] sSyukkabi = Get_bumonsyukka(sUser, conn2, sData[0], sData[1]);
				sRet[0] = sSyukkabi[0];
				if(sRet[0] != " ") return sRet;
				if(int.Parse(sData[2]) < int.Parse(sSyukkabi[1]))
				{
					sRet[0] = "1";
					sRet[1] = sSyukkabi[1];
					return sRet;
				}

				//�ב��l�b�c���݃`�F�b�N
				string cmdQuery
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
////					= "SELECT NVL(COUNT(*),0) \n"
////					= "SELECT COUNT(*) \n"
//// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
////					= "SELECT COUNT(ROWID) \n"
//					= "SELECT ���Ӑ�b�c, ���Ӑ敔�ۂb�c \n"
//// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END
//					+ "  FROM �r�l�O�P�ב��l \n"
//					+ " WHERE ����b�c   = '" + sData[0]  +"' \n"
//					+ "   AND ����b�c   = '" + sData[1]  +"' \n"
//					+ "   AND �ב��l�b�c = '" + sData[15] +"' \n"
//					+ "   AND �폜�e�f   = '0'";
					= "SELECT SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c \n"
					+ "     , NVL(CM01.�ۗ�����e�f,'0') \n"
					+ "  FROM �r�l�O�P�ב��l SM01 \n"
					+ "     , �b�l�O�P��� CM01 \n"
					+ " WHERE SM01.����b�c   = '" + sData[0]  +"' \n"
					+ "   AND SM01.����b�c   = '" + sData[1]  +"' \n"
					+ "   AND SM01.�ב��l�b�c = '" + sData[15] +"' \n"
					+ "   AND SM01.�폜�e�f   = '0' \n"
					+ "   AND SM01.����b�c   = CM01.����b�c(+) \n"
					;
				string s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END


				// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
				//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

				cmdQuery
					= "SELECT SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c \n"
					+ "     , NVL(CM01.�ۗ�����e�f,'0') \n"
					+ "  FROM �r�l�O�P�ב��l SM01 \n"
					+ "     , �b�l�O�P��� CM01 \n"
					+ " WHERE SM01.����b�c   = :p_KaiinCD \n"
					+ "   AND SM01.����b�c   = :p_BumonCD \n"
					+ "   AND SM01.�ב��l�b�c = :p_NisouCD \n"
					+ "   AND SM01.�폜�e�f   = '0' \n"
					+ "   AND SM01.����b�c   = CM01.����b�c(+) \n"
					;
				wk_opOraParam = new OracleParameter[3];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0],  ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1],  ParameterDirection.Input);
				wk_opOraParam[2] = new OracleParameter("p_NisouCD", OracleDbType.Char, sData[15], ParameterDirection.Input);

				OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
//				reader.Read();
//				d����   = reader.GetDecimal(0);
				if(reader.Read()){
					d���� = 1;
					sData[16] = reader.GetString(0);
					sData[17] = reader.GetString(1);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(2);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				}else{
					d���� = 0;
				}
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if(d���� == 0)
				{
					sRet[0] = "0";
				}
				else
				{
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
					cmdQuery
						= "SELECT SM04.���Ӑ敔�ۖ� \n"
						+ " FROM �b�l�O�Q���� CM02 \n"
						+    " , �r�l�O�S������ SM04 \n"
						+ " WHERE CM02.����b�c = '" + sData[0] + "' \n"
						+  " AND CM02.����b�c = '" + sData[1] + "' \n"
						+  " AND CM02.�폜�e�f = '0' \n"
//						+  " AND SM04.����b�c = CM02.����b�c \n"
						+  " AND SM04.�X�֔ԍ� = CM02.�X�֔ԍ� \n"
						+  " AND SM04.���Ӑ�b�c = '" + sData[16] + "' \n"
						+  " AND SM04.���Ӑ敔�ۂb�c = '" + sData[17] + "' \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
						+  " AND SM04.����b�c = CM02.����b�c \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
						+  " AND SM04.�폜�e�f = '0' \n"
						;
					// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
					//reader = CmdSelect(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

					cmdQuery
						= "SELECT SM04.���Ӑ敔�ۖ� \n"
						+ " FROM �b�l�O�Q���� CM02 \n"
						+    " , �r�l�O�S������ SM04 \n"
						+ " WHERE CM02.����b�c       = :p_KaiinCD \n"
						+  "  AND CM02.����b�c       = :p_BumonCD \n"
						+  "  AND CM02.�폜�e�f       = '0' \n"
						+  "  AND SM04.�X�֔ԍ�       = CM02.�X�֔ԍ� \n"
						+  "  AND SM04.���Ӑ�b�c     = :p_TokuiCD \n"
						+  "  AND SM04.���Ӑ敔�ۂb�c = :p_TokuiBukaCD \n"
						+  "  AND SM04.����b�c       = CM02.����b�c \n"
						+  "  AND SM04.�폜�e�f       = '0' \n"
						;
					wk_opOraParam = new OracleParameter[4];
					wk_opOraParam[0] = new OracleParameter("p_KaiinCD",     OracleDbType.Char, sData[0],  ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_BumonCD",     OracleDbType.Char, sData[1],  ParameterDirection.Input);
					wk_opOraParam[2] = new OracleParameter("p_TokuiCD",     OracleDbType.Char, sData[16], ParameterDirection.Input);
					wk_opOraParam[3] = new OracleParameter("p_TokuiBukaCD", OracleDbType.Char, sData[17], ParameterDirection.Input);

					reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

					if(reader.Read())
					{
						sData[18] = reader.GetString(0);
					}else{
						sData[18] = " ";
					}
					disposeReader(reader);
					reader = null;
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END

					//����v�擾
					if(sData[4] != " ")
					{
						cmdQuery
							= "SELECT NVL(����v,' ') \n"
							+ "  FROM �r�l�O�Q�׎�l \n"
							+ " WHERE ����b�c   = '" + sData[0] +"' \n"
							+ "   AND ����b�c   = '" + sData[1] +"' \n"
							+ "   AND �׎�l�b�c = '" + sData[4] +"' \n"
							+ "   AND �폜�e�f   = '0'";

						// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
						//reader = CmdSelect(sUser, conn2, cmdQuery);
						logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

						cmdQuery
							= "SELECT NVL(����v,' ') \n"
							+ "  FROM �r�l�O�Q�׎�l \n"
							+ " WHERE ����b�c   = :p_KaiinCD \n"
							+ "   AND ����b�c   = :p_BumonCD \n"
							+ "   AND �׎�l�b�c = :p_NiukeCD \n"
							+ "   AND �폜�e�f   = '0'";

						wk_opOraParam = new OracleParameter[3];
						wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
						wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);
						wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sData[4], ParameterDirection.Input);

						reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
						wk_opOraParam = null;
						// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

						bool bRead = reader.Read();
						if(bRead == true)
							s����v   = reader.GetString(0);

// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V START
						cmdQuery
							= "UPDATE �r�l�O�Q�׎�l \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V START
//							+ " SET ���O�R = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET �o�^�o�f = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V END
							+ " WHERE ����b�c = '" + sData[0] +"' \n"
							+ " AND ����b�c   = '" + sData[1] +"' \n"
							+ " AND �׎�l�b�c = '" + sData[4] +"' \n"
							+ " AND �폜�e�f   = '0'";
						try{
							// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
							//int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
							logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

							cmdQuery
								= "UPDATE �r�l�O�Q�׎�l \n"
								+ " SET �o�^�o�f = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
								+ " WHERE ����b�c = :p_KaiinCD \n"
								+ " AND ����b�c   = :p_BumonCD \n"
								+ " AND �׎�l�b�c = :p_NiukeCD \n"
								+ " AND �폜�e�f   = '0'";

							wk_opOraParam	= new OracleParameter[3];
							wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
							wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);
							wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sData[4], ParameterDirection.Input);

							iUpdRow = CmdUpdate(sUser, conn2, cmdQuery, wk_opOraParam);
							wk_opOraParam = null;
							// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
						}catch(Exception){
							;
						}
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V END
					}

//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
					//�W���[�i���m�n�擾
					cmdQuery
						= "SELECT \"�W���[�i���m�n�o�^��\",\"�W���[�i���m�n�Ǘ�\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM �b�l�O�Q���� \n"
						+ " WHERE ����b�c = '" + sData[0] +"' \n"
						+ "   AND ����b�c = '" + sData[1] +"' \n"
// MOD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
//						+ "   AND �폜�e�f = '0'";
						+ "   AND �폜�e�f = '0'"
						+ "   FOR UPDATE "
						;
// MOD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

					// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
					//reader = CmdSelect(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

					cmdQuery
						= "SELECT \"�W���[�i���m�n�o�^��\",\"�W���[�i���m�n�Ǘ�\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM �b�l�O�Q���� \n"
						+ " WHERE ����b�c = :p_KaiinCD \n"
						+ "   AND ����b�c = :p_BumonCD \n"
						+ "   AND �폜�e�f = '0'"
						+ "   FOR UPDATE "
						;
					wk_opOraParam = new OracleParameter[2];
					wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sData[0], ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sData[1], ParameterDirection.Input);

					reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

					reader.Read();
					s�o�^��   = reader.GetString(0).Trim();
					i�Ǘ��m�n = reader.GetInt32(1);
					s���t     = reader.GetString(2);

//					s���t = DateTime.Today.ToString().Replace("/","").Substring(0,8);
					if(s�o�^�� == s���t)
						i�Ǘ��m�n++;
					else
					{
						s�o�^�� = s���t;
						i�Ǘ��m�n = 1;
					}

					cmdQuery 
						= "UPDATE �b�l�O�Q���� \n"
						+    "SET \"�W���[�i���m�n�o�^��\"  = '" + s�o�^�� +"', \n"
						+        "\"�W���[�i���m�n�Ǘ�\"    = " + i�Ǘ��m�n +", \n"
						+        "�X�V�o�f                  = '" + sData[32] +"', \n"
						+        "�X�V��                    = '" + sData[33] +"', \n"
						+        "�X�V����                  =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE ����b�c       = '" + sData[0] +"' \n"
						+ "   AND ����b�c       = '" + sData[1] +"' \n"
						+ "   AND �폜�e�f = '0'";

					// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
					//int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

					cmdQuery 
						= "UPDATE �b�l�O�Q���� \n"
						+    "SET \"�W���[�i���m�n�o�^��\"  = :p_TourokuBi, \n"
						+        "\"�W���[�i���m�n�Ǘ�\"    = :p_KanriNo, \n"
						+        "�X�V�o�f                 = :p_KoushinPG, \n"
						+        "�X�V��                   = :p_KoushinSha, \n"
						+        "�X�V����                 =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE ����b�c       = :p_KaiinCD \n"
						+ "   AND ����b�c       = :p_BumonCD \n"
						+ "   AND �폜�e�f = '0'";

					wk_opOraParam	= new OracleParameter[6];
					wk_opOraParam[0] = new OracleParameter("p_TourokuBi",  OracleDbType.Char,    s�o�^��,   ParameterDirection.Input);
					wk_opOraParam[1] = new OracleParameter("p_KanriNo",    OracleDbType.Decimal, i�Ǘ��m�n, ParameterDirection.Input);
					wk_opOraParam[2] = new OracleParameter("p_KoushinPG",  OracleDbType.Char,    sData[32], ParameterDirection.Input);
					wk_opOraParam[3] = new OracleParameter("p_KoushinSha", OracleDbType.Char,    sData[33], ParameterDirection.Input);
					wk_opOraParam[4] = new OracleParameter("p_KaiinCD",    OracleDbType.Char,    sData[0],  ParameterDirection.Input);
					wk_opOraParam[5] = new OracleParameter("p_BumonCD",    OracleDbType.Char,    sData[1],  ParameterDirection.Input);

					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery, wk_opOraParam);
					wk_opOraParam = null;
					// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
//					string[] sRet2 = Get_JurnalNo(sUser, sData[0], sData[1], sData[32]);
//					if(sRet2[0].Length == 4){
//						s�o�^��   = sRet2[1];
//						s���t     = sRet2[1];
//						i�Ǘ��m�n = int.Parse(sRet2[2]);
//					}else{
//						tran.Rollback();
//						return sRet2;
//					}
//					int iUpdRow = 1;
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� END

					//���X�擾
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�������@�̍ĕύX START
//// MOD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX START
////					string[] sTyakuten = Get_tyakuten(sUser, conn2, sData[13] + sData[14]);
//					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
//						sData[0], sData[1], sData[4], 
//						sData[13] + sData[14], sData[8] + sData[9] + sData[10]);
//// MOD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX END
					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
						sData[0], sData[1], sData[4], 
						sData[13] + sData[14], sData[8] + sData[9] + sData[10], sData[11] + sData[12]);
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�������@�̍ĕύX END
					sRet[0] = sTyakuten[0];
					if(sRet[0] != " ") return sRet;
// MOD 2008.10.16 kcl)�X�{ ���X�R�[�h���� Empty �ɂȂ�Ȃ��悤�ɂ��� START
//					string s���X�b�c = sTyakuten[1];
//					string s���X��   = sTyakuten[2];
//					string s�Z���b�c = sTyakuten[3];
					string s���X�b�c = (sTyakuten[1].Length > 0) ? sTyakuten[1] : " ";
					string s���X��   = (sTyakuten[2].Length > 0) ? sTyakuten[2] : " ";
					string s�Z���b�c = (sTyakuten[3].Length > 0) ? sTyakuten[3] : " ";
// MOD 2008.10.16 kcl)�X�{ ���X�R�[�h���� Empty �ɂȂ�Ȃ��悤�ɂ��� END
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� START
					if(sData[8].Equals("�����x�X�~�߁���"))
					{
						// ���X�R�[�h�ϊ�
						string s�ϊ��㒅�X�b�c = ���p�����ϊ�(sData[10]);
						sRet[0] = s�ϊ��㒅�X�b�c;
						if(sRet[0].Length != 3)
						{
							// ���p�ϊ����s
							return sRet;
						}

						// ���X����
						string[] sTyakuten_GeneralDelivery = Get_tyakuten_GeneralDelivery(sUser, conn2, s�ϊ��㒅�X�b�c, sData[13] + sData[14]);
						sRet[0] = sTyakuten_GeneralDelivery[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s���X�b�c = s�ϊ��㒅�X�b�c;
						s���X��   = (sTyakuten_GeneralDelivery[1].Length > 0) ? sTyakuten_GeneralDelivery[1] : " ";
					}
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� END

					//���X�擾
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� START
//					string[] sHatuten = Get_hatuten(sData[15]);
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� START
					string[] sHatuten = Get_hatuten(sUser, conn2, sData[0], sData[1]);
//					string[] sHatuten = Get_hatuten3(sUser, conn2, sData[0], sData[1], sData[15]);
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� END
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� END
					sRet[0] = sHatuten[0];
					if(sRet[0] != " ") return sRet;
					string s���X�b�c = sHatuten[1];
					string s���X��   = sHatuten[2];

					//�W�דX�擾
					string[] sSyuyaku = Get_syuuyakuten(sUser, conn2, sData[0], sData[1]);
					sRet[0] = sSyuyaku[0];
					if(sRet[0] != " ") return sRet;
					string s�W��X�b�c = sSyuyaku[1];

// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
					//�Г��`����̏ꍇ�A���X�ƏW��X�����X���p�ɍX�V
					if(sData[0].Substring(0,2).ToUpper() == "FK")
					{
						// ���X�A�W��X����
						string[] sHatuten_HouseSlip = Get_hatuten_HouseSlip(sUser, conn2, sData[0]);
						sRet[0] = sHatuten_HouseSlip[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s���X�b�c   = (sHatuten_HouseSlip[1].Length > 0) ? sHatuten_HouseSlip[1] : " ";
						s���X��     = (sHatuten_HouseSlip[2].Length > 0) ? sHatuten_HouseSlip[2] : " ";
						s�W��X�b�c = (sHatuten_HouseSlip[3].Length > 0) ? sHatuten_HouseSlip[3] : " ";
					}
// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END

// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
					//�d���b�c�擾
					string s�d���b�c = " ";
					if(s���X�b�c.Trim().Length > 0 && s���X�b�c.Trim().Length > 0){
						string[] sRetSiwake = Get_siwake(sUser, conn2, s���X�b�c, s���X�b�c);
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j END
//						if(sRet[0] != " ") return sRet;
						s�d���b�c = sRetSiwake[1];
					}
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//					// �����O�Q�ɍː�����яd�ʂ̎Q�l�l������
//					string s�ː� = "";
//					string s�d�� = "";
//					string s�ː��d�� = "";
//					try{
//						s�ː� = sData[38].Trim().PadLeft(5,'0');
//						s�d�� = sData[20].Trim().PadLeft(5,'0');
//						s�ː��d�� = s�ː�.Substring(0,5)
//									+ s�d��.Substring(0,5);
//					}catch(Exception){
//					}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
///					string s�d�ʓ��͐��� = (sData.Length > 42) ? sData[42] : "0";
///					if(s�d�ʓ��͐��� != "1"){
///					string s�d�ʓ��͐��� = (sData.Length > 42) ? sData[42] : " ";
					if(s�d�ʓ��͐��� == "0"){
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						sData[38] = "0"; // �ː�
						sData[20] = "0"; // �d��
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					string s�i���L���S = (sData.Length > 43) ? sData[43] : " ";
					string s�i���L���T = (sData.Length > 44) ? sData[44] : " ";
					string s�i���L���U = (sData.Length > 45) ? sData[45] : " ";
					if(s�i���L���S.Length == 0) s�i���L���S = " ";
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					cmdQuery 
						= "INSERT INTO \"�r�s�O�P�o�׃W���[�i��\" \n"
// MOD 2010.10.13 ���s�j���� [�i���L���S]�ȂǍ��ڒǉ� START
						+ "(����b�c, ����b�c, �o�^��, \"�W���[�i���m�n\", �o�ד� \n"
						+ ", ���q�l�o�הԍ�, �׎�l�b�c \n"
						+ ", �d�b�ԍ��P, �d�b�ԍ��Q, �d�b�ԍ��R, �e�`�w�ԍ��P, �e�`�w�ԍ��Q, �e�`�w�ԍ��R \n"
						+ ", �Z���b�c, �Z���P, �Z���Q, �Z���R \n"
						+ ", ���O�P, ���O�Q, ���O�R \n"
						+ ", �X�֔ԍ� \n"
						+ ", ���X�b�c, ���X��, ����v \n"
						+ ", �ב��l�b�c, �ב��l������ \n"
						+ ", �W��X�b�c, ���X�b�c, ���X�� \n"
						+ ", ���Ӑ�b�c, ���ۂb�c, ���ۖ� \n"
						+ ", ��, �ː�, �d��, ���j�b�g \n"
						+ ", �w���, �w����敪 \n"
						+ ", �A���w���b�c�P, �A���w���P \n"
						+ ", �A���w���b�c�Q, �A���w���Q \n"
						+ ", �i���L���P, �i���L���Q, �i���L���R \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
						+ ", �i���L���S, �i���L���T, �i���L���U \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
						+ ", �����敪, �ی����z, �^��, ���p, ������ \n"
						+ ", �d���b�c, �����ԍ�, �����敪 \n"
						+ ", ����󔭍s�ςe�f, �o�׍ςe�f, ���M�ςe�f, �ꊇ�o�ׂe�f \n"
						+ ", ���, �ڍ׏�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
						+ ", �����O�Q \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�דo�^���́A�����O�S�Ɂu0�v��ݒ肷��
						+ ",�����O�S \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
						+ ", �폜�e�f, �o�^����, �o�^�o�f, �o�^�� \n"
						+ ", �X�V����, �X�V�o�f, �X�V�� \n"
						+ ") \n"
// MOD 2010.10.13 ���s�j���� [�i���L���S]�ȂǍ��ڒǉ� END
						+ "VALUES ('" + sData[0] +"','" + sData[1] +"','" + s���t +"'," + i�Ǘ��m�n +",'" + sData[2] +"', \n"
						+         "'" + sData[3] +"','" + sData[4] +"', \n"
						+         "'" + sData[5] +"','" + sData[6] +"','" + sData[7] +"',' ',' ',' ', \n"
						+         "'" + s�Z���b�c +"','" + sData[8] +"','" + sData[9] +"','" + sData[10] +"', \n"
						+         "'" + sData[11] +"','" + sData[12] +"',' ', \n"
						+         "'" + sData[13] + sData[14] +"', \n"
						+         "'" + s���X�b�c +"','" + s���X�� + "','" + s����v +"', \n"        //���X�b�c�@���X���@����v
						+         "'" + sData[15] +"','" + sData[37] +"', \n"						  // �ב��l�b�c  �ב��l������
						+         "'" + s�W��X�b�c + "','" + s���X�b�c + "','" + s���X�� + "', \n"  //�W��X�b�c�@���X�b�c�@���X��
						+         "'" + sData[16] +"','" + sData[17] +"','" + sData[18] +"', \n"
						+         "" + sData[19] +"," + sData[38] +"," + sData[20] +",0, \n"
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
// MOD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� START
//						+         "'" + sData[21] +"','" + sData[22] +"','" + sData[23] +"', \n"
//						+         "'" + sData[21] +"','" + sData[39] +"','" + sData[22] +"','" + sData[40] +"','" + sData[23] +"', \n"
						+         "'" + sData[21] +"','" + sData[41] +"', \n"
						+         "'" + sData[39] +"','" + sData[22] +"', \n"
						+         "'" + sData[40] +"','" + sData[23] +"', \n"
// MOD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� END
// MOD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
						+         "'" + sData[24] +"','" + sData[25] +"','" + sData[26] +"', \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
						+         "'" + s�i���L���S +"','"+ s�i���L���T +"','"+ s�i���L���U +"', \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
						+         "'" + sData[27] +"'," + sData[28] +",0,0,0, \n"  //�^���@���p�@������
// MOD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
//						+         "' ',' ',' ',"  //  �d���b�c  �����ԍ�  �����敪
						+         "'" + s�d���b�c + "',' ',' ',"  //  �d���b�c  �����ԍ�  �����敪
// MOD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END
						+         "'" + sData[29] +"','" + sData[30] +"', '0', '" + sData[31] +"', \n"  //   ���M�ςe�f
						+         "'01','  ', \n"        //��ԁ@�ڍ׏��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//						+         "'" + s�ː��d�� + "', \n" // �����O�Q
						+         "' ', \n" // �����O�Q
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�דo�^���́A�����O�S�Ɂu0�v��ݒ肷��
						+         "'0', \n" // �����O�S
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
						+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[32] +"','" + sData[33] +"', \n"
						+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[32] +"','" + sData[33] +"')";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�o�דo�^["+sData[1]+"]["+s���t+"]["+i�Ǘ��m�n+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

					// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
					//		MOD-S 2012.10.01 COA)���R Oracle�T�[�o���׌y���΍�iORA-01461�ɂ��AST01�ւ�INSERT�����̃R�[�h�ɖ߂��j
					/**/
					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					/**/
					/*
					logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

					cmdQuery 
						= "INSERT INTO \"�r�s�O�P�o�׃W���[�i��\" \n"
						+ "(����b�c, ����b�c, �o�^��, \"�W���[�i���m�n\", �o�ד� \n"
						+ ", ���q�l�o�הԍ�, �׎�l�b�c \n"
						+ ", �d�b�ԍ��P, �d�b�ԍ��Q, �d�b�ԍ��R, �e�`�w�ԍ��P, �e�`�w�ԍ��Q, �e�`�w�ԍ��R \n"
						+ ", �Z���b�c, �Z���P, �Z���Q, �Z���R \n"
						+ ", ���O�P, ���O�Q, ���O�R \n"
						+ ", �X�֔ԍ� \n"
						+ ", ���X�b�c, ���X��, ����v \n"
						+ ", �ב��l�b�c, �ב��l������ \n"
						+ ", �W��X�b�c, ���X�b�c, ���X�� \n"
						+ ", ���Ӑ�b�c, ���ۂb�c, ���ۖ� \n"
						+ ", ��, �ː�, �d��, ���j�b�g \n"
						+ ", �w���, �w����敪 \n"
						+ ", �A���w���b�c�P, �A���w���P \n"
						+ ", �A���w���b�c�Q, �A���w���Q \n"
						+ ", �i���L���P, �i���L���Q, �i���L���R \n"
						+ ", �i���L���S, �i���L���T, �i���L���U \n"
						+ ", �����敪, �ی����z, �^��, ���p, ������ \n"
						+ ", �d���b�c, �����ԍ�, �����敪 \n"
						+ ", ����󔭍s�ςe�f, �o�׍ςe�f, ���M�ςe�f, �ꊇ�o�ׂe�f \n"
						+ ", ���, �ڍ׏�� \n"
						+ ", �����O�Q \n"
						+ ", �폜�e�f, �o�^����, �o�^�o�f, �o�^�� \n"
						+ ", �X�V����, �X�V�o�f, �X�V�� \n"
						+ ") \n"
						+ "VALUES (:p_KaiinCD, :p_BumonCD, :p_TourokuBi, :p_JournalNo, :p_SyukkaBi, \n"
						+         ":p_CstmSyukkaNo, :p_NiukeCD, \n"
						+         ":p_TelNo_1, :p_TelNo_2, :p_TelNo_3, ' ', ' ', ' ', \n"
						+         ":p_AddrCD, :p_Addr_1, :p_Addr_2, :p_Addr_3, \n"
						+         ":p_Name_1, :p_Name_2, ' ', \n"
						+         ":p_YuubinNo, \n"
						+         ":p_ChakutenCD, :p_ChakutenName, :p_TokushuKei, \n"		//���X�b�c�@���X���@����v
						+         ":p_NiokuriCD, :p_NiokuriBusho, \n"						// �ב��l�b�c  �ב��l������
						+         ":p_ShuuyakutenCD, :p_HatsutenCD, :p_HatsutenName, \n"	//�W��X�b�c�@���X�b�c�@���X��
						+         ":p_TokuiCD, :p_TokuiBukaCD, :p_TokuiBukaName, \n"
						+         ":p_Kosuu, :p_Saisuu, :p_Juuryou, 0, \n"
						+         ":p_ShiteiBi, :p_ShiteiBiKBN, \n"
						+         ":p_YusoushijiCD_1, :p_Yusoushiji_1, \n"
						+         ":p_YusoushijiCD_2, :p_Yusoushiji_2, \n"
						+         ":p_Kiji_1, :p_Kiji_2, :p_Kiji_3, \n"
						+         ":p_Kiji_4, :p_Kiji_5, :p_Kiji_6, \n"
						+         ":p_MotoChakuKBN, :p_Hoken, 0, 0, 0, \n"						//�^���@���p�@������
						+         ":p_ShiwakeCD, ' ', ' ', "									//  �d���b�c  �����ԍ�  �����敪
						+         ":p_HakkouSumiFG, :p_SyukkaSumiFG, '0', :p_IsseiSyukkaFG, \n"	//   ���M�ςe�f
						+         "'01','  ', \n"        //��ԁ@�ڍ׏��
						+         "' ', \n" // �����O�Q
						+         "'0', TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), :p_TourokuPG, :p_Tourokusha, \n"
						+         "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), :p_KoushinPG, :p_Koushinsha)";

					wk_opOraParam	= new OracleParameter[53];
					wk_opOraParam[0]  = new OracleParameter("p_KaiinCD",        OracleDbType.Char,    sData[0],    ParameterDirection.Input);
					wk_opOraParam[1]  = new OracleParameter("p_BumonCD",        OracleDbType.Char,    sData[1],    ParameterDirection.Input);
					wk_opOraParam[2]  = new OracleParameter("p_TourokuBi",      OracleDbType.Char,    s���t,       ParameterDirection.Input);
					wk_opOraParam[3]  = new OracleParameter("p_JournalNo",      OracleDbType.Decimal, i�Ǘ��m�n,   ParameterDirection.Input);
					wk_opOraParam[4]  = new OracleParameter("p_SyukkaBi",       OracleDbType.Char,    sData[2],    ParameterDirection.Input);
					wk_opOraParam[5]  = new OracleParameter("p_CstmSyukkaNo",   OracleDbType.Char,    sData[3],    ParameterDirection.Input);
					wk_opOraParam[6]  = new OracleParameter("p_NiukeCD",        OracleDbType.Char,    sData[4],    ParameterDirection.Input);
					wk_opOraParam[7]  = new OracleParameter("p_TelNo_1",        OracleDbType.Char,    sData[5],    ParameterDirection.Input);
					wk_opOraParam[8]  = new OracleParameter("p_TelNo_2",        OracleDbType.Char,    sData[6],    ParameterDirection.Input);
					wk_opOraParam[9]  = new OracleParameter("p_TelNo_3",        OracleDbType.Char,    sData[7],    ParameterDirection.Input);
					wk_opOraParam[10] = new OracleParameter("p_AddrCD",         OracleDbType.Char,    s�Z���b�c,   ParameterDirection.Input);
					wk_opOraParam[11] = new OracleParameter("p_Addr_1",         OracleDbType.Char,    sData[8],    ParameterDirection.Input);
					wk_opOraParam[12] = new OracleParameter("p_Addr_2",         OracleDbType.Char,    sData[9],    ParameterDirection.Input);
					wk_opOraParam[13] = new OracleParameter("p_Addr_3",         OracleDbType.Char,    sData[10],   ParameterDirection.Input);
					wk_opOraParam[14] = new OracleParameter("p_Name_1",         OracleDbType.Char,    sData[11],   ParameterDirection.Input);
					wk_opOraParam[15] = new OracleParameter("p_Name_2",         OracleDbType.Char,    sData[12],   ParameterDirection.Input);
					wk_opOraParam[16] = new OracleParameter("p_YuubinNo",       OracleDbType.Char,    sData[13]+sData[14], ParameterDirection.Input);
					wk_opOraParam[17] = new OracleParameter("p_ChakutenCD",     OracleDbType.Char,    s���X�b�c,   ParameterDirection.Input);
					wk_opOraParam[18] = new OracleParameter("p_ChakutenName",   OracleDbType.Char,    s���X��,     ParameterDirection.Input);
					wk_opOraParam[19] = new OracleParameter("p_TokushuKei",     OracleDbType.Char,    s����v,     ParameterDirection.Input);
					wk_opOraParam[20] = new OracleParameter("p_NiokuriCD",      OracleDbType.Char,    sData[15],   ParameterDirection.Input);
					wk_opOraParam[21] = new OracleParameter("p_NiokuriBusho",   OracleDbType.Char,    sData[37],   ParameterDirection.Input);
					wk_opOraParam[22] = new OracleParameter("p_ShuuyakutenCD",  OracleDbType.Char,    s�W��X�b�c, ParameterDirection.Input);
					wk_opOraParam[23] = new OracleParameter("p_HatsutenCD",     OracleDbType.Char,    s���X�b�c,   ParameterDirection.Input);
					wk_opOraParam[24] = new OracleParameter("p_HatsutenName",   OracleDbType.Char,    s���X��,     ParameterDirection.Input);
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
					wk_opOraParam[40] = new OracleParameter("p_Kiji_4",         OracleDbType.Char,    s�i���L���S, ParameterDirection.Input);
					wk_opOraParam[41] = new OracleParameter("p_Kiji_5",         OracleDbType.Char,    s�i���L���T, ParameterDirection.Input);
					wk_opOraParam[42] = new OracleParameter("p_Kiji_6",         OracleDbType.Char,    s�i���L���U, ParameterDirection.Input);
					wk_opOraParam[43] = new OracleParameter("p_MotoChakuKBN",   OracleDbType.Char,    sData[27],   ParameterDirection.Input);
					wk_opOraParam[44] = new OracleParameter("p_Hoken",          OracleDbType.Decimal, sData[28],   ParameterDirection.Input);
					wk_opOraParam[45] = new OracleParameter("p_ShiwakeCD",      OracleDbType.Char,    s�d���b�c,   ParameterDirection.Input);
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
					//		MOD-E 2012.10.01 COA)���R Oracle�T�[�o���׌y���΍�iORA-01461�ɂ��AST01�ւ�INSERT�����̃R�[�h�ɖ߂��j
					// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

					tran.Commit();
					sRet[0] = "����I��";
					sRet[1] = s���t;
					sRet[2] = i�Ǘ��m�n.ToString();
				}

			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� START
				if(ex.Number == 1438){ // ORA-01438: value larger than specified precision allows for this column
//					if(i�Ǘ��m�n > 9999){
						sRet[0] = "�P���ň�����o�א��i9999���j���z���܂����B";
//					}
				}
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� END
			}
			catch (Exception ex)
			{
				tran.Rollback();
				string sErr = ex.Message.Substring(0,9);
				if(sErr == "ORA-00001")
					sRet[0] = "����̃R�[�h�����ɑ��̒[�����o�^����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
				else
					sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �o�׃f�[�^�X�V
		 * �����F����b�c�A����b�c�A�o�ד�...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_syukka(string[] sUser, string[] sData)
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
		{
			return Upd_syukka2(sUser, sData, "-");
		}
		[WebMethod]
		public String[] Upd_syukka2(string[] sUser, string[] sData, string sNo)
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׍X�V�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			decimal d����;
			string s����v = " ";
			try
			{
				//�o�ד��`�F�b�N
				string[] sSyukkabi = Get_bumonsyukka(sUser, conn2, sData[0], sData[1]);
				sRet[0] = sSyukkabi[0];
				if(sRet[0] != " ") return sRet;
				if(int.Parse(sData[2]) < int.Parse(sSyukkabi[1]))
				{
					sRet[0] = "1";
					sRet[1] = sSyukkabi[1];
					return sRet;
				}

				//�ב��l�b�c���݃`�F�b�N
				string cmdQuery
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
////					= "SELECT NVL(COUNT(*),0) \n"
////					= "SELECT COUNT(*) \n"
//// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
////					= "SELECT COUNT(ROWID) \n"
//					= "SELECT ���Ӑ�b�c, ���Ӑ敔�ۂb�c \n"
//// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END
//					+ "  FROM �r�l�O�P�ב��l \n"
//					+ " WHERE ����b�c   = '" + sData[0]  +"' \n"
//					+ "   AND ����b�c   = '" + sData[1]  +"' \n"
//					+ "   AND �ב��l�b�c = '" + sData[15] +"' \n"
//					+ "   AND �폜�e�f   = '0'";
					= "SELECT SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c \n"
					+ "     , NVL(CM01.�ۗ�����e�f,'0') \n"
					+ "  FROM �r�l�O�P�ב��l SM01 \n"
					+ "     , �b�l�O�P��� CM01 \n"
					+ " WHERE SM01.����b�c   = '" + sData[0]  +"' \n"
					+ "   AND SM01.����b�c   = '" + sData[1]  +"' \n"
					+ "   AND SM01.�ב��l�b�c = '" + sData[15] +"' \n"
					+ "   AND SM01.�폜�e�f   = '0' \n"
					+ "   AND SM01.����b�c   = CM01.����b�c(+) \n"
					;
				string s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
//				reader.Read();
//				d����   = reader.GetDecimal(0);
				if(reader.Read()){
					d���� = 1;
					sData[16] = reader.GetString(0);
					sData[17] = reader.GetString(1);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(2);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				}else{
					d���� = 0;
				}
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				if(d���� == 0)
				{
					sRet[0] = "0";
				}
				else
				{
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 START
					cmdQuery
						= "SELECT SM04.���Ӑ敔�ۖ� \n"
						+ " FROM �b�l�O�Q���� CM02 \n"
						+    " , �r�l�O�S������ SM04 \n"
						+ " WHERE CM02.����b�c = '" + sData[0] + "' \n"
						+  " AND CM02.����b�c = '" + sData[1] + "' \n"
						+  " AND CM02.�폜�e�f = '0' \n"
						+  " AND SM04.����b�c = CM02.����b�c \n"
						+  " AND SM04.�X�֔ԍ� = CM02.�X�֔ԍ� \n"
						+  " AND SM04.���Ӑ�b�c = '" + sData[16] + "' \n"
						+  " AND SM04.���Ӑ敔�ۂb�c = '" + sData[17] + "' \n"
						+  " AND SM04.�폜�e�f = '0' \n"
						;
					reader = CmdSelect(sUser, conn2, cmdQuery);
					if(reader.Read()){
						sData[18] = reader.GetString(0);
					}else{
						sData[18] = " ";
					}
					disposeReader(reader);
					reader = null;
// MOD 2008.07.03 ���s�j���� ���Ӑ���̍Ď擾 END

					//����v�擾
					if(sData[4] != " ")
					{
						cmdQuery
							= "SELECT NVL(����v,' ') \n"
							+ "  FROM �r�l�O�Q�׎�l \n"
							+ " WHERE ����b�c   = '" + sData[0] +"' \n"
							+ "   AND ����b�c   = '" + sData[1] +"' \n"
							+ "   AND �׎�l�b�c = '" + sData[4] +"' \n"
							+ "   AND �폜�e�f   = '0'";

						reader = CmdSelect(sUser, conn2, cmdQuery);

						bool bRead = reader.Read();
						if(bRead == true)
							s����v   = reader.GetString(0);

// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader);
						reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V START
						cmdQuery
							= "UPDATE �r�l�O�Q�׎�l \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V START
//							+ " SET ���O�R = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET �o�^�o�f = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V END
							+ " WHERE ����b�c = '" + sData[0] +"' \n"
							+ " AND ����b�c   = '" + sData[1] +"' \n"
							+ " AND �׎�l�b�c = '" + sData[4] +"' \n"
							+ " AND �폜�e�f   = '0'";
						try{
							int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
						}catch(Exception){
							;
						}
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V END
					}

					//���X�擾
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�������@�̍ĕύX START
//// MOD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX START
////					string[] sTyakuten = Get_tyakuten(sUser, conn2, sData[13] + sData[14]);
//					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
//						sData[0], sData[1], sData[4], 
//						sData[13] + sData[14], sData[8] + sData[9] + sData[10]);
//// MOD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX END
					string[] sTyakuten = Get_tyakuten3(sUser, conn2, 
						sData[0], sData[1], sData[4], 
						sData[13] + sData[14], sData[8] + sData[9] + sData[10], sData[11] + sData[12]);
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�������@�̍ĕύX END
					sRet[0] = sTyakuten[0];
					if(sRet[0] != " ") return sRet;
// MOD 2008.10.16 kcl)�X�{ ���X�R�[�h���� Empty �ɂȂ�Ȃ��悤�ɂ��� START
//					string s���X�b�c = sTyakuten[1];
//					string s���X��   = sTyakuten[2];
//					string s�Z���b�c = sTyakuten[3];
					string s���X�b�c = (sTyakuten[1].Length > 0) ? sTyakuten[1] : " ";
					string s���X��   = (sTyakuten[2].Length > 0) ? sTyakuten[2] : " ";
					string s�Z���b�c = (sTyakuten[3].Length > 0) ? sTyakuten[3] : " ";
// MOD 2008.10.16 kcl)�X�{ ���X�R�[�h���� Empty �ɂȂ�Ȃ��悤�ɂ��� END
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� START
					if(sData[8].Equals("�����x�X�~�߁���"))
					{
						// ���X�R�[�h�ϊ�
						string s�ϊ��㒅�X�b�c = ���p�����ϊ�(sData[10]);
						sRet[0] = s�ϊ��㒅�X�b�c;
						if(sRet[0].Length != 3)
						{
							// ���p�ϊ����s
							return sRet;
						}

						// ���X����
						string[] sTyakuten_GeneralDelivery = Get_tyakuten_GeneralDelivery(sUser, conn2, s�ϊ��㒅�X�b�c, sData[13] + sData[14]);
						sRet[0] = sTyakuten_GeneralDelivery[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s���X�b�c = s�ϊ��㒅�X�b�c;
						s���X��   = (sTyakuten_GeneralDelivery[1].Length > 0) ? sTyakuten_GeneralDelivery[1] : " ";
					}
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� END

					//���X�擾
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� START
//					string[] sHatuten = Get_hatuten(sData[15]);
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� START
					string[] sHatuten = Get_hatuten(sUser, conn2, sData[0], sData[1]);
//					string[] sHatuten = Get_hatuten3(sUser, conn2, sData[0], sData[1], sData[15]);
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� END
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� END
					sRet[0] = sHatuten[0];
					if(sRet[0] != " ") return sRet;
					string s���X�b�c = sHatuten[1];
					string s���X��   = sHatuten[2];

					//�W�דX�擾
					string[] sSyuyaku = Get_syuuyakuten(sUser, conn2, sData[0], sData[1]);
					sRet[0] = sSyuyaku[0];
					if(sRet[0] != " ") return sRet;
					string s�W��X�b�c = sSyuyaku[1];

// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
					//�Г��`����̏ꍇ�A���X�ƏW��X�����X���p�ɍX�V
					if(sData[0].Substring(0,2).ToUpper() == "FK")
					{
						// ���X�A�W��X����
						string[] sHatuten_HouseSlip = Get_hatuten_HouseSlip(sUser, conn2, sData[0]);
						sRet[0] = sHatuten_HouseSlip[0];
						if(sRet[0] != " ")
						{
							return sRet;
						}
						s���X�b�c   = (sHatuten_HouseSlip[1].Length > 0) ? sHatuten_HouseSlip[1] : " ";
						s���X��     = (sHatuten_HouseSlip[2].Length > 0) ? sHatuten_HouseSlip[2] : " ";
						s�W��X�b�c = (sHatuten_HouseSlip[3].Length > 0) ? sHatuten_HouseSlip[3] : " ";
					}
// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END

// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
					//�d���b�c�擾
					string s�d���b�c = " ";
					if(s���X�b�c.Trim().Length > 0 && s���X�b�c.Trim().Length > 0){
						string[] sRetSiwake = Get_siwake(sUser, conn2, s���X�b�c, s���X�b�c);
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j END
//						if(sRet[0] != " ") return sRet;
						s�d���b�c = sRetSiwake[1];
					}
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END

// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
///					string s�d�ʓ��͐��� = (sData.Length > 42) ? sData[42] : "0";
///					if(s�d�ʓ��͐��� != "1"){
///					string s�d�ʓ��͐��� = (sData.Length > 42) ? sData[42] : " ";
					if(s�d�ʓ��͐��� == "0"){
						sData[38] = "0"; // �ː�
						sData[20] = "0"; // �d��
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					string s�i���L���S = (sData.Length > 43) ? sData[43] : " ";
					string s�i���L���T = (sData.Length > 44) ? sData[44] : " ";
					string s�i���L���U = (sData.Length > 45) ? sData[45] : " ";
					if(s�i���L���S.Length == 0) s�i���L���S = " ";
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END

					cmdQuery 
						= "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n"
						+    "SET �o�ד�             = '" + sData[2]  +"', \n"
						+        "���q�l�o�הԍ�     = '" + sData[3]  +"',"
						+        "�׎�l�b�c         = '" + sData[4]  +"',"
						+        "�d�b�ԍ��P         = '" + sData[5]  +"', \n"
						+        "�d�b�ԍ��Q         = '" + sData[6]  +"',"
						+        "�d�b�ԍ��R         = '" + sData[7]  +"',"
						+        "�Z���b�c           = '" + s�Z���b�c +"', \n"
						+        "�Z���P             = '" + sData[8]  +"',"
						+        "�Z���Q             = '" + sData[9]  +"',"
						+        "�Z���R             = '" + sData[10] +"', \n"
						+        "���O�P             = '" + sData[11] +"',"
						+        "���O�Q             = '" + sData[12] +"',"
						+        "�X�֔ԍ�           = '" + sData[13] + sData[14] +"', \n"
						+        "���X�b�c           = '" + s���X�b�c +"',"
						+        "���X��             = '" + s���X��   +"',"
						+        "����v             = '" + s����v   +"', \n"
						+        "�ב��l�b�c         = '" + sData[15] +"',"
						+        "�ב��l������       = '" + sData[37] +"',"
						+        "�W��X�b�c         = '" + s�W��X�b�c +"', \n"
						+        "���X�b�c           = '" + s���X�b�c +"',"
						+        "���X��             = '" + s���X��   +"',"
						+        "���Ӑ�b�c         = '" + sData[16] +"', \n"
						+        "���ۂb�c           = '" + sData[17] +"',"
						+        "���ۖ�             = '" + sData[18] +"',"
						+        "��               =  " + sData[19] +", \n"
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� START
						+        "�ː�               =  " + sData[38] +","
// MOD 2005.05.17 ���s�j�����J �ː��ǉ� END
						+        "�d��               =  " + sData[20] +","
						+        "�w���             = '" + sData[21] +"',"
// ADD 2005.06.01 ���s�j�ɉ� �w����敪�ǉ� START
						+        "�w����敪         = '" + sData[41] +"',"
// ADD 2005.06.01 ���s�j�ɉ� �w����敪�ǉ� END
// ADD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� START
						+        "�A���w���b�c�P     = '" + sData[39] +"',"
// ADD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� END
						+        "�A���w���P         = '" + sData[22] +"', \n"
// ADD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� START
						+        "�A���w���b�c�Q     = '" + sData[40] +"',"
// ADD 2005.06.01 ���s�j�ɉ� �A�����i�R�[�h�ǉ� END
						+        "�A���w���Q         = '" + sData[23] +"',"
						+        "�i���L���P         = '" + sData[24] +"',"
						+        "�i���L���Q         = '" + sData[25] +"', \n"
						+        "�i���L���R         = '" + sData[26] +"',"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
						+        "�i���L���S         = '" + s�i���L���S +"', \n"
						+        "�i���L���T         = '" + s�i���L���T +"',"
						+        "�i���L���U         = '" + s�i���L���U +"', \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
						+        "�ی����z           =  " + sData[28] +","
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
						+        "�d���b�c           = '" + s�d���b�c + "', \n"
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END
						+        "����󔭍s�ςe�f   = '0', \n"
						+        "���M�ςe�f         = '0',"
//						+        "���               = DECODE(���,'03','02','01'),"
						+        "���               = '01',"
						+        "�ڍ׏��           = '  ', \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�׍X�V���i�폜�������l�j�́A�����O�S�Ɂu1�v��ݒ肷��
						+        "�����O�S           = '1',"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
						+        "�X�V�o�f           = '" + sData[32] +"',"
						+        "�X�V��             = '" + sData[33] +"', \n"
						+        "�X�V����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE ����b�c           = '" + sData[0]  +"' \n"
						+ "   AND ����b�c           = '" + sData[1]  +"' \n"
						+ "   AND �o�^��             = '" + sData[35] +"' \n"
						+ "   AND \"�W���[�i���m�n\" = '" + sData[34] +"' \n"
						+ "   AND �X�V����           =  " + sData[36] +"";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�o�׍X�V["+sData[1]+"]["+sData[35]+"]["+sData[34]+"]:["+sNo+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					tran.Commit();
					if(iUpdRow == 0)
						sRet[0] = "�f�[�^�ҏW���ɑ��̒[�����X�V����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
					else
						sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �o�׃f�[�^�폜
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n�A�X�V�o�f�A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_syukka(string[] sUser, string[] sData)
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
		{
			return Del_syukka2(sUser, sData, "-");
		}
		[WebMethod]
		public String[] Del_syukka2(string[] sUser, string[] sData, string sNo)
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׍폜�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery 
					= "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n"
					+    "SET ���M�ςe�f         = '0', \n"
					+       " �폜�e�f           = '1', \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
					//�o�׍폜���i�X�V�������l�j�́A�����O�S�Ɂu1�v��ݒ肷��
					+        "�����O�S           = '1', \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
// MOD 2010.10.27 ���s�j���� �폜�����Ȃǂ̒ǉ� START
//// MOD 2009.09.11 ���s�j���� �o�׏Ɖ�ŏo�׍ςe�f,���M�ςe�f�Ȃǂ�ǉ� START
//					+        "�o�^����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
////					+        "�X�V�o�f           = '" + sData[4] +"', \n"
////					+        "�X�V��             = '" + sData[5] +"', \n"
////					+        "�X�V����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
//// MOD 2009.09.11 ���s�j���� �o�׏Ɖ�ŏo�׍ςe�f,���M�ςe�f�Ȃǂ�ǉ� END
					+        "�폜�o�f           = '" + sData[4] +"', \n"
					+        "�폜��             = '" + sData[5] +"', \n"
					+        "�폜����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
// MOD 2010.10.27 ���s�j���� �폜�����Ȃǂ̒ǉ� END
					+ " WHERE ����b�c           = '" + sData[0] +"' \n"
					+ "   AND ����b�c           = '" + sData[1] +"' \n"
					+ "   AND �o�^��             = '" + sData[2] +"' \n"
					+ "   AND \"�W���[�i���m�n\" = '" + sData[3] +"'";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�o�׍폜["+sData[1]+"]["+sData[2]+"]["+sData[3]+"]:["+sNo+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//�ۗ��@�폜�Ȃ̂Ŕr���`�F�b�N���Ȃ��i�폜�D��j
				tran.Commit();				
				sRet[0] = "����I��";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * �o�׃f�[�^�ꊇ�폜
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n�A�X�V�o�f�A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Del_ikkatu(string[] sUser, string[] sData, string[] sInday, string[] sNo)
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
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
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׈ꊇ�폜�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery; 
				for(int iCnt = 0; iCnt < sInday.Length && sNo[iCnt] != ""; iCnt++)
				{
					cmdQuery
						= "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n"
						+    "SET ���M�ςe�f         = '0', \n"
						+       " �폜�e�f           = '1', \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�׍폜���i�X�V�������l�j�́A�����O�S�Ɂu1�v��ݒ肷��
						+        "�����O�S           = '1', \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
// MOD 2010.10.27 ���s�j���� �폜�����Ȃǂ̒ǉ� START
//// MOD 2009.09.11 ���s�j���� �o�׏Ɖ�ŏo�׍ςe�f,���M�ςe�f�Ȃǂ�ǉ� START
//						+        "�o�^����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'), \n"
////						+        "�X�V�o�f           = '" + sData[2] +"', \n"
////						+        "�X�V��             = '" + sData[3] +"', \n"
////						+        "�X�V����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
//// MOD 2009.09.11 ���s�j���� �o�׏Ɖ�ŏo�׍ςe�f,���M�ςe�f�Ȃǂ�ǉ� END
						+        "�폜�o�f           = '" + sData[2] +"', \n"
						+        "�폜��             = '" + sData[3] +"', \n"
						+        "�폜����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
// MOD 2010.10.27 ���s�j���� �폜�����Ȃǂ̒ǉ� END
						+ " WHERE ����b�c           = '" + sData[0] +"' \n"
						+ "   AND ����b�c           = '" + sData[1] +"' \n"
						+ "   AND �o�^��             = '" + sInday[iCnt] +"' \n"
						+ "   AND \"�W���[�i���m�n\" = '" + sNo[iCnt] +"'";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�o�׈ꊇ�폜["+sData[1]+"]["+sInday[iCnt]+"]["+sNo[iCnt]+"]:["+sONo[iCnt]+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//�ۗ��@�폜�Ȃ̂Ŕr���`�F�b�N���Ȃ��i�폜�D��j
				}
				tran.Commit();				
				sRet[0] = "����I��";
				
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * ���X�擾
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�s���{���b�c�A�s�撬���b�c�A�厚�ʏ̂b�c
		 *
		 *********************************************************************/
		private String[] Get_tyakuten(string[] sUser, OracleConnection conn2, string sYuubin)
		{
			string[] sRet = new string[4];

/*			string cmdQuery = "SELECT T.�X���b�c,T.�X���� "
				+ " FROM �b�l�P�S�X�֔ԍ� Y,�b�l�P�R�Z�� J,�b�l�P�O�X�� T"
				+ " WHERE Y.�X�֔ԍ� = '" + sYuubin + "'"
				+ "   AND Y.�s���{���b�c = J.�s���{���b�c"
				+ "   AND Y.�s�撬���b�c = J.�s�撬���b�c"
				+ "   AND Y.�厚�ʏ̂b�c = J.�厚�ʏ̂b�c"
				+ "   AND Y.�폜�e�f     = '0'"
				+ "   AND J.�X���b�c     = T.�X���b�c"
				+ "   AND J.�폜�e�f     = '0'"
				+ "   AND T.�폜�e�f     = '0'";
*/
			string cmdQuery
				= "SELECT T.�X���b�c,T.�X����,Y.�s���{���b�c || Y.�s�撬���b�c || Y.�厚�ʏ̂b�c \n"
				+ " FROM �b�l�P�S�X�֔ԍ� Y,�b�l�P�O�X�� T \n"
				+ " WHERE Y.�X�֔ԍ� = '" + sYuubin + "' \n"
				+ "   AND Y.�폜�e�f     = '0' \n"
				+ "   AND Y.�X���b�c     = T.�X���b�c \n"
				+ "   AND T.�폜�e�f     = '0'";

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
				sRet[0] = "���͂��ꂽ���͂���(�X�֔ԍ�)�ł͔z�B�X�����߂��܂���ł���";
				sRet[1] = "0000";
				sRet[2] = " ";
				sRet[3] = " ";
			}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			
			return sRet;
		}

		/*********************************************************************
		 * ���X�擾
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�s���{���b�c�A�s�撬���b�c�A�厚�ʏ̂b�c
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_tyakuten2(string[] sUser, string sYuubin)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���X�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery
					= "SELECT T.�X���b�c,T.�X����,Y.�s���{���b�c || Y.�s�撬���b�c || Y.�厚�ʏ̂b�c \n"
					+ " FROM �b�l�P�S�X�֔ԍ� Y,�b�l�P�O�X�� T \n"
					+ " WHERE Y.�X�֔ԍ� = '" + sYuubin + "' \n"
					+ "   AND Y.�폜�e�f     = '0' \n"
					+ "   AND Y.�X���b�c     = T.�X���b�c \n"
					+ "   AND T.�폜�e�f     = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					for(int iCnt = 1; iCnt < 4; iCnt++)
					{
						sRet[iCnt] = reader.GetString(iCnt - 1).Trim();
					}
					sRet[0] = "����I��";
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * ���X�擾
		 * �����F�ב��l�b�c
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�s���{���b�c�A�s�撬���b�c�A�厚�ʏ̂b�c
		 *
		 *********************************************************************/
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� START
//		private String[] Get_hatuten(string sIcode)
		private String[] Get_hatuten(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� END
		{
			string[] sRet = new string[4];
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j


/*			string cmdQuery = "SELECT T.�X���b�c,T.�X���� "
				+ " FROM �r�l�O�P�ב��l N,�b�l�P�S�X�֔ԍ� Y,�b�l�P�R�Z�� J,�b�l�P�O�X�� T"
				+ " WHERE N.�ב��l�b�c   = '" + sIcode + "'"
				+ "   AND N.�X�֔ԍ�     = Y.�X�֔ԍ�"
				+ "   AND N.�폜�e�f     = '0'"
				+ "   AND Y.�s���{���b�c = J.�s���{���b�c"
				+ "   AND Y.�s�撬���b�c = J.�s�撬���b�c"
				+ "   AND Y.�厚�ʏ̂b�c = J.�厚�ʏ̂b�c"
				+ "   AND Y.�폜�e�f     = '0'"
				+ "   AND J.�X���b�c     = T.�X���b�c"
				+ "   AND J.�폜�e�f     = '0'"
				+ "   AND T.�폜�e�f     = '0'";
*/
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� START
//			string cmdQuery
//				= "SELECT T.�X���b�c,T.�X����,Y.�s���{���b�c || Y.�s�撬���b�c || Y.�厚�ʏ̂b�c \n"
//				+ " FROM �r�l�O�P�ב��l N,�b�l�P�S�X�֔ԍ� Y,�b�l�P�O�X�� T \n"
//				+ " WHERE N.�ב��l�b�c   = '" + sIcode + "' \n"
//				+ "   AND N.�X�֔ԍ�     = Y.�X�֔ԍ� \n"
//				+ "   AND N.�폜�e�f     = '0' \n"
//				+ "   AND Y.�폜�e�f     = '0' \n"
//				+ "   AND Y.�X���b�c     = T.�X���b�c \n"
//				+ "   AND T.�폜�e�f     = '0'";
			string cmdQuery = "SELECT Y.�X���b�c, T.�X����, Y.�s���{���b�c, Y.�s�撬���b�c, Y.�厚�ʏ̂b�c \n"
				+ " FROM �b�l�O�Q���� B, \n"
				+      " �b�l�P�S�X�֔ԍ� Y, \n"
				+      " �b�l�P�O�X�� T \n"
				+ " WHERE B.����b�c = '" + sKcode + "' \n"
				+ " AND B.����b�c = '" + sBcode + "' \n"
				+ " AND B.�폜�e�f = '0' \n"
				+ " AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
				+ " AND Y.�X���b�c = T.�X���b�c \n"
				+ " AND T.�폜�e�f = '0' \n";
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� END

			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery = "SELECT Y.�X���b�c, T.�X����, Y.�s���{���b�c, Y.�s�撬���b�c, Y.�厚�ʏ̂b�c \n"
				+ " FROM �b�l�O�Q���� B, \n"
				+      " �b�l�P�S�X�֔ԍ� Y, \n"
				+      " �b�l�P�O�X�� T \n"
				+ " WHERE B.����b�c = :p_KaiinCD \n"
				+ " AND B.����b�c = :p_BumonCD \n"
				+ " AND B.�폜�e�f = '0' \n"
				+ " AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
				+ " AND Y.�X���b�c = T.�X���b�c \n"
				+ " AND T.�폜�e�f = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			bool bRead = reader.Read();
			if(bRead == true)
			{
// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� START
//				for(int iCnt = 1; iCnt < 4; iCnt++)
//				{
//					sRet[iCnt] = reader.GetString(iCnt - 1).Trim();
//				}
				sRet[1] = reader.GetString(0).Trim(); // �X���b�c
				sRet[2] = reader.GetString(1).Trim(); // �X����
				sRet[3] = reader.GetString(2).Trim()  // �Z���b�c
						+ reader.GetString(3).Trim()
						+ reader.GetString(4).Trim();

// MOD 2005.05.11 ���s�j���� ���X�̎擾���@�̏C�� END
				sRet[0] = " ";
			}
			else
			{
				sRet[0] = "���X�����߂��܂���ł���";
				sRet[1] = "0000";
				sRet[2] = " ";
				sRet[3] = " ";
			}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			
			return sRet;
		}

		/*********************************************************************
		 * ���X�擾
		 * �����F�ב��l�b�c
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�s���{���b�c�A�s�撬���b�c�A�厚�ʏ̂b�c
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_hatuten2(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���X�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT Y.�X���b�c, T.�X����, Y.�s���{���b�c, Y.�s�撬���b�c, Y.�厚�ʏ̂b�c \n"
					+ " FROM �b�l�O�Q���� B, \n"
					+      " �b�l�P�S�X�֔ԍ� Y, \n"
					+      " �b�l�P�O�X�� T \n"
					+ " WHERE B.����b�c = '" + sKcode + "' \n"
					+ " AND B.����b�c = '" + sBcode + "' \n"
					+ " AND B.�폜�e�f = '0' \n"
					+ " AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
					+ " AND Y.�X���b�c = T.�X���b�c \n"
					+ " AND T.�폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim()
						+ reader.GetString(3).Trim()
						+ reader.GetString(4).Trim();

					sRet[0] = "����I��";
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� START
//		private String[] Get_hatuten3(string[] sUser, OracleConnection conn2
//										, string sKcode, string sBcode, string sIcode)
//		{
//			string[] sRet = new string[4];
//			sRet = Get_hatuten(sUser, conn2, sKcode, sBcode);
//			if(sRet[1] != "030") return sRet;
//
//			string cmdQuery
//				= "SELECT NVL(CM14.�X���b�c,' ') \n"
//				+ ", NVL(CM10.�X����,' ') \n"
//				+ ", NVL(CM14.�s���{���b�c,' ') \n"
//				+ ", NVL(CM14.�s�撬���b�c,' ') \n"
//				+ ", NVL(CM14.�厚�ʏ̂b�c,' ') \n"
//				+ "  FROM �r�l�O�P�ב��l SM01 \n"
//				+ ", �b�l�P�S�X�֔ԍ� CM14 \n"
//				+ ", �b�l�P�O�X�� CM10 \n"
//				+ " WHERE SM01.����b�c   = '" + sKcode +"' \n"
//				+ "   AND SM01.����b�c   = '" + sBcode +"' \n"
//				+ "   AND SM01.�ב��l�b�c = '" + sIcode +"' \n"
//				+ "   AND SM01.�X�֔ԍ�   = CM14.�X�֔ԍ�(+)"
//				+ "   AND CM14.�X���b�c   = CM10.�X���b�c(+)"
//				;
//
//			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
//			if(reader.Read()){
//				sRet[1] = reader.GetString(0).Trim(); // �X���b�c
//				sRet[2] = reader.GetString(1).Trim(); // �X����
//				sRet[3] = reader.GetString(2).Trim()  // �Z���b�c
//						+ reader.GetString(3).Trim()
//						+ reader.GetString(4).Trim();
//
//				sRet[0] = " ";
//			}else{
//				sRet[0] = "���X�����߂��܂���ł���";
//				sRet[1] = "0000";
//				sRet[2] = " ";
//				sRet[3] = " ";
//			}
//			disposeReader(reader);
//			reader = null;
//
//			return sRet;
//		}
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� END
		/*********************************************************************
		 * �W��X�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�W��X�b�c
		 *
		 *********************************************************************/
		private String[] Get_syuuyakuten(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
		{
			string[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			string cmdQuery = "SELECT T.�W��X�b�c \n"
				+ " FROM �b�l�O�Q���� B,�b�l�P�O�X�� T, \n"
				+        "�b�l�P�S�X�֔ԍ� Y  \n"
				+ " WHERE B.����b�c   = '" + sKcode + "' \n"
				+ "   AND B.����b�c   = '" + sBcode + "' \n"
				+ "   AND B.�폜�e�f     = '0' \n"
				+    "AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
				+    "AND Y.�X���b�c     = T.�X���b�c \n"
				+ "   AND T.�폜�e�f     = '0'";

			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery = "SELECT T.�W��X�b�c \n"
				+ " FROM �b�l�O�Q���� B,�b�l�P�O�X�� T, \n"
				+        "�b�l�P�S�X�֔ԍ� Y  \n"
				+ " WHERE B.����b�c   = :p_KaiinCD \n"
				+ "   AND B.����b�c   = :p_BumonCD \n"
				+ "   AND B.�폜�e�f     = '0' \n"
				+    "AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
				+    "AND Y.�X���b�c     = T.�X���b�c \n"
				+ "   AND T.�폜�e�f     = '0'";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			bool bRead = reader.Read();
			if(bRead == true)
			{
				sRet[0] = " ";
				sRet[1] = reader.GetString(0).Trim();
			}
			else
			{
				sRet[0] = "�W��X�����߂��܂���ł���";
				sRet[1] = "0000";
			}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

			return sRet;
		}

		/*********************************************************************
		 * �W��X�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�W��X�b�c
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syuuyakuten2(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�W��X�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT T.�W��X�b�c \n"
					+ " FROM �b�l�O�Q���� B,�b�l�P�O�X�� T, \n"
					+        "�b�l�P�S�X�֔ԍ� Y  \n"
					+ " WHERE B.����b�c   = '" + sKcode + "' \n"
					+ "   AND B.����b�c   = '" + sBcode + "' \n"
					+ "   AND B.�폜�e�f     = '0' \n"
					+    "AND B.�X�֔ԍ� = Y.�X�֔ԍ� \n"
					+    "AND Y.�X���b�c     = T.�X���b�c \n"
					+ "   AND T.�폜�e�f     = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
		/*********************************************************************
		 * �d���b�c�擾
		 * �����F����b�c�A����b�c�A�c�a�ڑ��A���X�A���X
		 * �ߒl�F�X�e�[�^�X�A�d���b�c
		 *
		 *********************************************************************/
		private static string GET_SIWAKE_SELECT
			= "SELECT �d���b�c \n"
			+ " FROM �b�l�P�V�d�� \n"
			;

		private String[] Get_siwake(string[] sUser, OracleConnection conn2, string sHatuCd, string sTyakuCd)
		{
//			logWriter(sUser, INF, "�d���b�c�擾�J�n");

			string[] sRet = new string[2];
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			string cmdQuery = GET_SIWAKE_SELECT
				+ " WHERE ���X���b�c = '" + sHatuCd + "' \n"
				+ " AND ���X���b�c = '" + sTyakuCd + "' \n"
				+ " AND �폜�e�f = '0' \n"
				;

			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��
			
			cmdQuery = GET_SIWAKE_SELECT
				+ " WHERE ���X���b�c = :p_HatsutenCD \n"
				+ "   AND ���X���b�c = :p_ChakutenCD \n"
				+ "   AND �폜�e�f = '0' \n"
				;
			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_HatsutenCD", OracleDbType.Char, sHatuCd,  ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_ChakutenCD", OracleDbType.Char, sTyakuCd, ParameterDirection.Input);

			OracleDataReader	reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			if(reader.Read())
			{
				sRet[0] = " ";
//				sRet[1] = reader.GetString(0).Trim();
				sRet[1] = reader.GetString(0);
			}
			else
			{
				sRet[0] = "�d���b�c�����߂��܂���ł���";
				sRet[1] = " ";
			}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

			return sRet;
		}
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END

		/*********************************************************************
		 * ����}�X�^�o�ד��擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�ד�
		 *
		 *********************************************************************/
		private String[] Get_bumonsyukka(string[] sUser, OracleConnection conn2, string sKcode, string sBcode)
		{
			string[] sRet = new string[2];

			string cmdQuery = "SELECT �o�ד� \n"
				+ " FROM �b�l�O�Q���� \n"
				+ " WHERE ����b�c   = '" + sKcode + "' \n"
				+ "   AND ����b�c   = '" + sBcode + "' \n"
				+ "   AND �폜�e�f   = '0' \n";

			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
			OracleParameter[]	wk_opOraParam	= null;

			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery = "SELECT �o�ד� \n"
				+ " FROM �b�l�O�Q���� \n"
				+ " WHERE ����b�c   = :p_KaiinCD \n"
				+ "   AND ����b�c   = :p_BumonCD \n"
				+ "   AND �폜�e�f   = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKcode, ParameterDirection.Input);
			wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBcode, ParameterDirection.Input);

			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			bool bRead = reader.Read();
			if(bRead == true)
			{
				sRet[0] = " ";
				sRet[1] = reader.GetString(0).Trim();
			}
			else
			{
				sRet[0] = "�o�ד��G���[";
				sRet[1] = "0";
			}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

			return sRet;
		}

		/*********************************************************************
		 * �o�׈ꗗ�擾�i�b�r�u�o�͗p�j
		 * �����F����b�c�A����b�c�A�׎�l�b�c�A�ב��l�b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I�����A���
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n�A�׎�l�b�c...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_3
// ADD 2005.06.03 ���s�j�����J �˗�����ǉ� START
//			= "SELECT J.�o�^��, TO_CHAR(\"�W���[�i���m�n\"), J.�׎�l�b�c, J.�X�֔ԍ�, \n"
//			+       " '(' || TRIM(J.�d�b�ԍ��P) || ')' || TRIM(J.�d�b�ԍ��Q) || '-' || J.�d�b�ԍ��R, \n"
//			+       " J.�Z���P, J.�Z���Q, J.�Z���R, J.���O�P, J.���O�Q, J.����v, J.���X�b�c, J.���X��, \n"
//			+       " J.�ב��l�b�c, TO_CHAR(J.��), TO_CHAR(J.�d��), \n"
//			+       " J.�w���, J.�A���w���P, J.�A���w���Q, J.�i���L���P, J.�i���L���Q, J.�i���L���R, \n"
//			+       " J.�����敪, TO_CHAR(J.�ی����z), J.���q�l�o�הԍ�, \n"
//			+       " J.�o�ד�, J.���Ӑ�b�c, J.���ۂb�c, \n"
////			+       " CASE �����ԍ� WHEN ' ' THEN ' ' "
////			+       " ELSE SUBSTR(�����ԍ�,5,3) || '-' || SUBSTR(�����ԍ�,8,4) || '-' || SUBSTR(�����ԍ�,12,4) END \n"
//			+       " �����ԍ� \n"
//			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J \n";

			= "SELECT /*+ INDEX(J ST01IDX2) INDEX(N SM01PKEY) */ \n"
			+       " J.�o�^��, J.�o�ד�, �����ԍ�, J.�׎�l�b�c, J.�X�֔ԍ�, \n"
			+       " '(' || TRIM(J.�d�b�ԍ��P) || ')' || TRIM(J.�d�b�ԍ��Q) || '-' || J.�d�b�ԍ��R, \n"
			+       " J.�Z���P, J.�Z���Q, J.�Z���R, J.���O�P, J.���O�Q, J.����v, J.���X�b�c, J.���X��, \n"
			+       " J.�ב��l�b�c, NVL(N.�X�֔ԍ�, ' '), \n"
			+       " NVL(N.�d�b�ԍ��P,' '), NVL(N.�d�b�ԍ��Q,' '), NVL(N.�d�b�ԍ��R,' '), \n"
			+       " NVL(N.�Z���P,' '), NVL(N.�Z���Q,' '), NVL(N.���O�P,' '), NVL(N.���O�Q,' '), \n"
			+       " TO_CHAR(J.��), TO_CHAR(J.�d��), \n"
			+       " J.�w���, J.�A���w���P, J.�A���w���Q, J.�i���L���P, J.�i���L���Q, J.�i���L���R, \n"
			+       " J.�����敪, TO_CHAR(J.�ی����z), J.���q�l�o�הԍ�, \n"
			+       " J.���Ӑ�b�c, J.���ۂb�c, J.�ː� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
			+       " , J.�^���ː�, J.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
			+       ", J.�ב��l������, J.�w����敪, J.�i���L���S, J.�i���L���T, J.�i���L���U \n"
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jSTART
			+       ", J.���X�b�c, J.���X�� \n"
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jEND
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� START
			+       ", DECODE(J.�����O�R,'          ',' ',('20' || SUBSTR(J.�����O�R,1,2) || '/' || SUBSTR(J.�����O�R,3,2) || '/' || SUBSTR(J.�����O�R,5,2) || ' ' || SUBSTR(J.�����O�R,7,2) || ':' || SUBSTR(J.�����O�R,9,2))) \n"
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J,�r�l�O�P�ב��l N \n"
// ADD 2005.06.03 ���s�j�����J �˗�����ǉ� START
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			;

		[WebMethod]
		public String[] Get_csvwrite(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
		{
			string[] sKey = new string[]{sKCode, sBCode, sTCode, sICode, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_csvwrite2(sUser, sKey);
		}

		[WebMethod]
		public String[] Get_csvwrite2(string[] sUser, string[] sKey)
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�b�r�u�o�͂Q�J�n");
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string sTCode   = sKey[2];
			string sICode   = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s�����ԍ��J�n = ""; if(sKey.Length >  8) s�����ԍ��J�n = sKey[ 8];
			string s�����ԍ��I�� = ""; if(sKey.Length >  9) s�����ԍ��I�� = sKey[ 9];
			string s���q�l�ԍ��J�n = ""; if(sKey.Length > 10) s���q�l�ԍ��J�n = sKey[10];
			string s���q�l�ԍ��I�� = ""; if(sKey.Length > 11) s���q�l�ԍ��I�� = sKey[11];
			string s������b�c     = ""; if(sKey.Length > 12) s������b�c     = sKey[12];
			string s�����敔�ۂb�c = ""; if(sKey.Length > 13) s�����敔�ۂb�c = sKey[13];
			int    i���[�o�͌`��   = 0 ; if(sKey.Length > 14) i���[�o�͌`��   = int.Parse(sKey[14]);
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jSTART
			string s�b�r�u�o�͌`�� = ""; if(sKey.Length > 15) s�b�r�u�o�͌`�� = sKey[15];
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jEND
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jSTART
			string s���X�b�c�o�͌`�� = ""; if(sKey.Length > 16) s���X�b�c�o�͌`�� = sKey[16];
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jEND
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� START
			string s�z���r�o�͌`�� = ""; if(sKey.Length > 17) s�z���r�o�͌`�� = sKey[17];
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� END

			if(s�����ԍ��J�n.Length == 0) s�����ԍ��J�n = s�����ԍ��I��;
			if(s�����ԍ��I��.Length == 0) s�����ԍ��I�� = s�����ԍ��J�n;
			if(s���q�l�ԍ��J�n.Length == 0) s���q�l�ԍ��J�n = s���q�l�ԍ��I��;
			if(s���q�l�ԍ��I��.Length == 0) s���q�l�ԍ��I�� = s���q�l�ԍ��J�n;
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː��d�� = 0;
			decimal d�d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			decimal d�ː� = 0;
//			string cmdQuery;
			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.����b�c = '" + sBCode + "' \n");
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
				if(s�����ԍ��J�n.Length > 0){
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX START
					if(s�����ԍ��J�n.Length >= 11){
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX END
						if(s�����ԍ��J�n == s�����ԍ��I��){
							sbQuery.Append(" AND J.�����ԍ� = '0000"+ s�����ԍ��J�n + "' \n");
						}else{
							sbQuery.Append(" AND J.�����ԍ� BETWEEN '0000"+ s�����ԍ��J�n
														 + "' AND '0000"+ s�����ԍ��I�� + "' \n");
						}
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX START
					}else if(s�����ԍ��J�n.Length >= 6){
						sbQuery.Append(" AND J.�����ԍ� BETWEEN '0000"+ s�����ԍ��J�n.PadRight(11,'0')
													 + "' AND '0000"+ s�����ԍ��I��.PadRight(11,'9') + "' \n");
					}else if(s�����ԍ��J�n.Length == 4
							|| s�����ԍ��J�n.Length == 5){
						if(s�����ԍ��J�n == s�����ԍ��I��){
							sbQuery.Append(" AND SUBSTR(J.�����ԍ�,"
							 +(5+11-s�����ԍ��J�n.Length)+","+s�����ԍ��J�n.Length
							 + ") = '" + s�����ԍ��J�n + "' \n");
						}else{
							sbQuery.Append(" AND SUBSTR(J.�����ԍ�,"
							 +(5+11-s�����ԍ��J�n.Length)+","+s�����ԍ��J�n.Length
							 + ") BETWEEN '" + s�����ԍ��J�n
							 + "' AND '"+ s�����ԍ��I�� + "' \n");
						}
					}else{
					}
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX END
				}
				if(s���q�l�ԍ��J�n.Length > 0){
					if(s���q�l�ԍ��J�n == s���q�l�ԍ��I��){
						sbQuery.Append(" AND J.���q�l�o�הԍ� = '"+ s���q�l�ԍ��J�n + "' \n");
					}else{
						sbQuery.Append(" AND J.���q�l�o�הԍ� BETWEEN '"+ s���q�l�ԍ��J�n
													 + "' AND '"+ s���q�l�ԍ��I�� + "' \n");
					}
				}
				if(s������b�c.Length > 0){
					sbQuery.Append(" AND J.���Ӑ�b�c = '"+ s������b�c + "' \n");
				}
				if(s�����敔�ۂb�c.Length > 0){
					sbQuery.Append(" AND J.���ۂb�c = '"+ s�����敔�ۂb�c + "' \n");
				}
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND J.�׎�l�b�c = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND J.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND J.�׎�l�b�c = '"+ sTCode + "' \n");
					sbQuery.Append(" AND J.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(iSyuka == 0)
					sbQuery.Append(" AND J.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sJyoutai != "00")
				{
// ADD 2008.07.09 ���s�j���� �����s�������O���� START
					if(sJyoutai == "aa")
						sbQuery.Append(" AND J.��� <> '01' \n");
					else
// ADD 2008.07.09 ���s�j���� �����s�������O���� END
						sbQuery.Append(" AND J.��� = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND J.�폜�e�f = '0' \n");
// ADD 2005.06.03 ���s�j�����J �˗�����擾 START
				sbQuery.Append(" AND J.�ב��l�b�c     = N.�ב��l�b�c(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '0' = N.�폜�e�f(+) ");
// ADD 2005.06.03 ���s�j�����J �˗�����擾 END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(" AND J.����b�c     = CM01.����b�c(+) \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

				OracleDataReader reader;
// MOD 2009.11.04 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
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
				switch(i���[�o�͌`��){
				case 1:		//���˗����
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY �o�ד�,�ב��l�b�c,�o�^��,\"�W���[�i���m�n\" ");
					}else{
						sbQuery2.Append(" ORDER BY �o�^��,�ב��l�b�c,\"�W���[�i���m�n\" ");
					}
					break;
				case 2:		//�������
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY �o�ד�,���Ӑ�b�c,���ۂb�c,�o�^��,\"�W���[�i���m�n\" ");
					}else{
						sbQuery2.Append(" ORDER BY �o�^��,���Ӑ�b�c,���ۂb�c,\"�W���[�i���m�n\" ");
					}
					break;
				case 3:		//���͂����
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY �o�ד�,�׎�l�b�c,�o�^��,\"�W���[�i���m�n\" ");
					}else{
						sbQuery2.Append(" ORDER BY �o�^��,�׎�l�b�c,\"�W���[�i���m�n\" ");
					}
					break;
				default:	//�w��Ȃ�
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
					}
					break;
				}
				reader = CmdSelect(sUser, conn2, sbQuery2);
// MOD 2009.11.04 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END

				StringBuilder sbData = new StringBuilder(1024);
				while (reader.Read())
				{
/*
					sList.Add(sDbl + reader.GetString(0).Trim() + sDbl              //�o�^��
						+ sKanma + sDbl + sSng + reader.GetString(1).Trim() + sDbl  //�W���[�i���m�n
						+ sKanma + sDbl + sSng + reader.GetString(2).Trim() + sDbl  //�׎�l�b�c
						+ sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl  //�X�֔ԍ�
						+ sKanma + sDbl + reader.GetString(4).Trim() + sDbl         //�d�b�ԍ�
						+ sKanma + sDbl + reader.GetString(5).Trim() + sDbl         //�Z���P
						+ sKanma + sDbl + reader.GetString(6).Trim() + sDbl         //�Z���Q
						+ sKanma + sDbl + reader.GetString(7).Trim() + sDbl         //�Z���R
						+ sKanma + sDbl + reader.GetString(8).Trim() + sDbl         //���O�P
						+ sKanma + sDbl + reader.GetString(9).Trim() + sDbl         //���O�Q
						+ sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl //����v
						+ sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl //���X�b�c
						+ sKanma + sDbl + reader.GetString(12).Trim() + sDbl        //���X��
						+ sKanma + sDbl + sSng + reader.GetString(13).Trim() + sDbl //�ב��l�b�c
						+ sKanma + reader.GetString(14)         //��
						+ sKanma + reader.GetString(15)         //�d��
						+ sKanma + sDbl + reader.GetString(16).Trim() + sDbl        //�w���
						+ sKanma + sDbl + sSng + reader.GetString(17).Trim() + sDbl //�A���w���P
						+ sKanma + sDbl + sSng + reader.GetString(18).Trim() + sDbl //�A���w���Q
						+ sKanma + sDbl + sSng + reader.GetString(19).Trim() + sDbl //�i���L���P
						+ sKanma + sDbl + sSng + reader.GetString(20).Trim() + sDbl //�i���L���Q
						+ sKanma + sDbl + sSng + reader.GetString(21).Trim() + sDbl //�i���L���R
						+ sKanma + sDbl + reader.GetString(22).Trim() + sDbl       //�����敪
						+ sKanma + reader.GetString(23)         //�ی����z
						+ sKanma + sDbl + sSng + reader.GetString(24).Trim() + sDbl //���q�l�o�הԍ�
						+ sKanma + sDbl + reader.GetString(25).Trim() + sDbl        //�o�ד�
						+ sKanma + sDbl + sSng + reader.GetString(26).Trim() + sDbl //���Ӑ�b�c
						+ sKanma + sDbl + sSng + reader.GetString(27).Trim() + sDbl //���ۂb�c
						+ sKanma + sDbl + reader.GetString(28).Trim() + sDbl        //�����ԍ�
						);
*/
					sbData = new StringBuilder(1024);
// MOD 2005.06.03 ���s�j�����J �˗�����ǉ� START
/*					sbData.Append(sDbl + sSng + reader.GetString(0).Trim() + sDbl);				// �o�^��
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(1).Trim() + sDbl);	// �W���[�i���m�n
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(2).Trim() + sDbl);	// �׎�l�b�c
					sbData.Append(sKanma + sDbl + reader.GetString(3).Trim() + sDbl);			// �X�֔ԍ�
					sbData.Append(sKanma + sDbl + reader.GetString(4).Trim() + sDbl);			// �d�b�ԍ�
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// �Z���P
					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// �Z���Q
					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// �Z���R
					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// ���O�P
					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// ���O�Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl);	// ����v
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// ���X�b�c
					sbData.Append(sKanma + sDbl + reader.GetString(12).Trim() + sDbl       );	// ���X��
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(13).Trim() + sDbl);	// �ב��l�b�c
					sbData.Append(sKanma + reader.GetString(14)                            );	// ��
					sbData.Append(sKanma + reader.GetString(15)                            );	// �d��
					sbData.Append(sKanma + sDbl + reader.GetString(16).Trim() + sDbl       );	// �w���
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(17).TrimEnd() + sDbl);	// �A���w���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(18).TrimEnd() + sDbl);	// �A���w���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(19).TrimEnd() + sDbl);	// �i���L���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(20).TrimEnd() + sDbl);	// �i���L���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(21).TrimEnd() + sDbl);	// �i���L���R
					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl       );	// �����敪
					sbData.Append(sKanma + reader.GetString(23)                            );	// �ی����z
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(24).Trim() + sDbl);	// ���q�l�o�הԍ�
					sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// �o�ד�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).Trim() + sDbl);	// ���Ӑ�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).Trim() + sDbl);	// ���ۂb�c
					string sNo = reader.GetString(28).Trim();									// �����ԍ�(XXX-XXXX-XXXX)
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
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jSTART
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//				if(s�b�r�u�o�͌`��.Equals("1")){
				if(s�b�r�u�o�͌`��.Equals("1") || s�b�r�u�o�͌`��.Equals("3")){
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					sbData.Append(sDbl + sSng + reader.GetString(3).Trim() + sDbl);		// �׎�l�b�c
//					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);	// �׎�l�d�b�ԍ�
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);	// �׎�l�Z���P
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);	// �׎�l�Z���Q
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);	// �׎�l�Z���R
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);	// �׎�l���O�P
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);	// �׎�l���O�Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(5).Trim() + sDbl);	// �׎�l�d�b�ԍ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(6).Trim() + sDbl);	// �׎�l�Z���P
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(7).Trim() + sDbl);	// �׎�l�Z���Q
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(8).Trim() + sDbl);	// �׎�l�Z���R
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(9).Trim() + sDbl);	// �׎�l���O�P
//					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).Trim() + sDbl);	// �׎�l���O�Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(6).TrimEnd() + sDbl);  // �׎�l�Z���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(7).TrimEnd() + sDbl);  // �׎�l�Z���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(8).TrimEnd() + sDbl);  // �׎�l�Z���R
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(9).TrimEnd() + sDbl);  // �׎�l���O�P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(10).TrimEnd() + sDbl); // �׎�l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// �׎�l�X�֔ԍ�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// ����v
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// ���X�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// �ב��l�b�c
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
				if(s�b�r�u�o�͌`��.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(40).TrimEnd() + sDbl); // �ב��S����
				}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					sbData.Append(sKanma + reader.GetString(23).Trim()                     );	// ��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					sbData.Append(sKanma + reader.GetDecimal(36).ToString().Trim()         );	// �ː�
//					sbData.Append(sKanma + reader.GetString(24).Trim()                     );	// �d��
					s�^���ː� = reader.GetString(37).TrimEnd();
					s�^���d�� = reader.GetString(38).TrimEnd();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(39).TrimEnd();
					if(s�d�ʓ��͐��� == "1"
					&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						sbData.Append(sKanma + reader.GetDecimal(36).ToString().TrimEnd());	// �ː�
						sbData.Append(sKanma + reader.GetString(24).TrimEnd());				// �d��
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						d�ː� = 0;
						d�d�� = 0;
						if(s�^���ː�.Length > 0){
							try{
								d�ː� = Decimal.Parse(s�^���ː�);
							}catch(Exception){}
						}
						if(s�^���d��.Length > 0){
							try{
								d�d�� = Decimal.Parse(s�^���d��);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d�ː�.ToString());		// �ː�
						sbData.Append(sKanma + d�d��.ToString());		// �d��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// �A���w���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// �A���w���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// �i���L���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// �i���L���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// �i���L���R
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
				if(s�b�r�u�o�͌`��.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(42).TrimEnd() + sDbl);	// �i���L���S
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(43).TrimEnd() + sDbl);	// �i���L���T
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(44).TrimEnd() + sDbl);	// �i���L���U
				}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END

					if(reader.GetString(25).Trim() == "0"){
						sbData.Append(sKanma + sDbl + sDbl);										// �w���
					}else{
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// �w���
					}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
				if(s�b�r�u�o�͌`��.Equals("3")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(41).TrimEnd() + sDbl); // �K���敪
				}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END

					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// ���q�l�o�הԍ�
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//					sbData.Append(sKanma + sDbl + sDbl);										// �\��
				if(s�b�r�u�o�͌`��.Equals("1")){
					sbData.Append(sKanma + sDbl + sDbl);										// �\��
				}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// �����敪
					sbData.Append(sKanma + reader.GetString(32).Trim()                     );	// �ی����z
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl);	// �o�ד�
					sbData.Append(sKanma + sDbl + sDbl);								// �o�^���i�ȗ��j
				}else{
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jEND
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F START
//					sbData.Append(sDbl + sSng + reader.GetString(0).Trim() + sDbl);				// �o�^��
					sbData.Append(sDbl + reader.GetString(0).Trim() + sDbl);					// �o�^��
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F END
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl       );	// �o�ד�
					string sNo = reader.GetString(2).Trim();									// �����ԍ�(XXX-XXXX-XXXX)
					if(sNo.Length == 15)
					{
						sbData.Append(sKanma + sDbl + sNo.Substring(4,3)
							+ "-" + sNo.Substring(7,4) + "-" + sNo.Substring(11) + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl);	// �׎�l�b�c
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F START
//					sbData.Append(sKanma + sDbl + reader.GetString(4).Trim() + sDbl);			// �׎�l�X�֔ԍ�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// �׎�l�X�֔ԍ�
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F END
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// �׎�l�d�b�ԍ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// �׎�l�Z���P
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// �׎�l�Z���Q
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// �׎�l�Z���R
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// �׎�l���O�P
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);			// �׎�l���O�Q
					sbData.Append(sKanma + sDbl + reader.GetString(6).TrimEnd() + sDbl);  // �׎�l�Z���P
					sbData.Append(sKanma + sDbl + reader.GetString(7).TrimEnd() + sDbl);  // �׎�l�Z���Q
					sbData.Append(sKanma + sDbl + reader.GetString(8).TrimEnd() + sDbl);  // �׎�l�Z���R
					sbData.Append(sKanma + sDbl + reader.GetString(9).TrimEnd() + sDbl);  // �׎�l���O�P
					sbData.Append(sKanma + sDbl + reader.GetString(10).TrimEnd() + sDbl); // �׎�l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// ����v
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// ���X�b�c
					sbData.Append(sKanma + sDbl + reader.GetString(13).Trim() + sDbl       );	// ���X��
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// �ב��l�b�c
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F START
//					sbData.Append(sKanma + sDbl + reader.GetString(15).Trim() + sDbl);			// �ב��l�X�֔ԍ�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(15).Trim() + sDbl);	// �ב��l�X�֔ԍ�
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F END

					string sTel = reader.GetString(16).Trim();									// �ב��l�d�b�ԍ�
					if(sTel.Length != 0)
					{
						sbData.Append(sKanma + sDbl + "(" + sTel + ")"
							+ "-" + reader.GetString(17).Trim() + "-" + reader.GetString(18).Trim() + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}

// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sbData.Append(sKanma + sDbl + reader.GetString(19).Trim() + sDbl);			// �ב��l�Z���P
//					sbData.Append(sKanma + sDbl + reader.GetString(20).Trim() + sDbl);			// �ב��l�Z���Q
//					sbData.Append(sKanma + sDbl + reader.GetString(21).Trim() + sDbl);			// �ב��l���O�P
//					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl);			// �ב��l���O�Q
					sbData.Append(sKanma + sDbl + reader.GetString(19).TrimEnd() + sDbl); // �ב��l�Z���P
					sbData.Append(sKanma + sDbl + reader.GetString(20).TrimEnd() + sDbl); // �ב��l�Z���Q
					sbData.Append(sKanma + sDbl + reader.GetString(21).TrimEnd() + sDbl); // �ב��l���O�P
					sbData.Append(sKanma + sDbl + reader.GetString(22).TrimEnd() + sDbl); // �ב��l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sbData.Append(sKanma + reader.GetString(23)                            );	// ��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					d�ː� = reader.GetDecimal(36);													// �ː�
//					d�ː� = d�ː� * 8;
//					if(d�ː� == 0)
//						sbData.Append(sKanma + reader.GetString(24)                            );	// �d��
//					else
//						sbData.Append(sKanma + d�ː�.ToString()                            );
					s�^���ː� = reader.GetString(37).TrimEnd();
					s�^���d�� = reader.GetString(38).TrimEnd();
					d�ː��d�� = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(39).TrimEnd();
					if(s�d�ʓ��͐��� == "1"
					&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						d�ː��d�� += (reader.GetDecimal(36) * 8);		// �ː�
						if(reader.GetString(24).TrimEnd().Length > 0){	// �d��
							try{
								d�ː��d�� += Decimal.Parse(reader.GetString(24).TrimEnd());
							}catch(Exception){}
						}
						sbData.Append(sKanma + d�ː��d��.ToString());	// �d��
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						if(s�^���ː�.Length > 0){
							try{
								d�ː��d�� += (Decimal.Parse(s�^���ː�) * 8);
							}catch(Exception){}
						}
						if(s�^���d��.Length > 0){
							try{
								d�ː��d�� += Decimal.Parse(s�^���d��);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d�ː��d��.ToString());		// �d��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F START
//					sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// �w���
					if(reader.GetString(25).Trim() == "0")
						sbData.Append(sKanma + sDbl + sDbl);										// �w���
					else
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// �w���
// MOD 2005.07.21 ���s�j���� �t�H�[�}�b�g�m�F END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// �A���w���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// �A���w���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// �i���L���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// �i���L���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// �i���L���R
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
				if(s�b�r�u�o�͌`��.Equals("2")){
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(42).TrimEnd() + sDbl);	// �i���L���S
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(43).TrimEnd() + sDbl);	// �i���L���T
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(44).TrimEnd() + sDbl);	// �i���L���U
				}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// �����敪
					sbData.Append(sKanma + reader.GetString(32)                            );	// �ی����z
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// ���q�l�o�הԍ�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(34).Trim() + sDbl);	// ���Ӑ�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(35).Trim() + sDbl);	// ���ۂb�c
// MOD 2005.06.03 ���s�j�����J �˗�����ǉ� END
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jSTART
				}
// MOD 2010.02.01 ���s�j���� �I�v�V�����̍��ڒǉ��i�b�r�u�o�͌`���jEND
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jSTART
				if(s���X�b�c�o�͌`��.Equals("1"))
				{
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(45).TrimEnd() + sDbl);	// ���X�b�c
					sbData.Append(sKanma + sDbl + reader.GetString(46).TrimEnd() + sDbl);	// ���X��
				}
// MOD 2013.04.04 TDI�j�j�V �o�̓��C�A�E�g�ǉ��i�O���[�o����p�jEND
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� START
				if(s�z���r�o�͌`��.Equals("1"))
				{
					sbData.Append(sKanma + sDbl + reader.GetString(47).Trim() + sDbl       );	// �z�����t�E����
				}
// MOD 2013.10.07 BEVAS�j���� �b�r�u�o�͂ɔz�����t�E������ǉ� END
					sList.Add(sbData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * �A�b�v���[�h�f�[�^�ǉ�
		 * �����F����b�c�A����b�c�A�o�ד�...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Ins_autoEntryData(string[] sUser, string[] sList)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�����o�דo�^�f�[�^�ǉ��J�n");

			OracleConnection conn2 = null;
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� START
//			string[] sRet = new string[1];
//			string s�X�V���� = System.DateTime.Now.ToString("yyyyMMddHHmmss");
			string[] sRet = new string[1 + sList.Length * 2];
			int iRetCnt = 1;
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

//�ۗ� ADD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
//			string s���X�b�c�� = "";
//			string s���X�b�c�� = "";
//			string s�d���b�c�� = " ";
//�ۗ� ADD 2007.04.27 ���s�j���� ORA-01000 �Ή� END
			sRet[0] = "";
			try
			{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				string s�d�ʓ��͐��� = " ";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				for (int i = 0; i < sList.Length; i++)
				{
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� START
					if(sList[i] == null) break;
					if(sList[i].Length == 0) break;
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� END
// DEL 2005.06.08 ���s�j���� ���g�p�̈׍폜 START
//					string s����v = " ";
// DEL 2005.06.08 ���s�j���� ���g�p�̈׍폜 END
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
					string s�o�^��;
					int i�Ǘ��m�n;
					string s���t;
					string[] sData = sList[i].Split(',');
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
///					string s�d�ʓ��͐��� = (sData.Length > 46) ? sData[46] : "0";
///					string s�d�ʓ��͐��� = (sData.Length > 46) ? sData[46] : " ";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
					string cmdQuery = "";
					OracleDataReader reader;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					if(s�d�ʓ��͐��� == " "){
						cmdQuery = "SELECT CM01.�ۗ�����e�f \n"
							+ "  FROM �b�l�O�P��� CM01 \n"
							+ " WHERE CM01.����b�c = '" + sData[0]  +"' \n"
							;
						reader = CmdSelect(sUser, conn2, cmdQuery);
						if(reader.Read()){
							s�d�ʓ��͐��� = reader.GetString(0);
						}
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

					//�W���[�i���m�n�擾
					cmdQuery
						= "SELECT \"�W���[�i���m�n�o�^��\",\"�W���[�i���m�n�Ǘ�\", \n"
						+ "       TO_CHAR(SYSDATE,'YYYYMMDD') \n"
						+ "  FROM �b�l�O�Q���� \n"
						+ " WHERE ����b�c = '" + sData[0] +"' \n"
						+ "   AND ����b�c = '" + sData[1] +"' \n"
						+ "   AND �폜�e�f = '0'"
						+ "   FOR UPDATE ";

					reader = CmdSelect(sUser, conn2, cmdQuery);
					reader.Read();
					s�o�^��   = reader.GetString(0).Trim();
					i�Ǘ��m�n = reader.GetInt32(1);
					s���t     = reader.GetString(2);
// ADD 2005.06.08 ���s�j�ɉ� ORA-01000�΍� START
					reader.Close();
// ADD 2005.06.08 ���s�j�ɉ� ORA-01000�΍� END
					if(s�o�^�� == s���t)
						i�Ǘ��m�n++;
					else
					{
						s�o�^�� = s���t;
						i�Ǘ��m�n = 1;
					}

					cmdQuery 
						= "UPDATE �b�l�O�Q���� \n"
						+    "SET \"�W���[�i���m�n�o�^��\"  = '" + s�o�^�� +"', \n"
						+        "\"�W���[�i���m�n�Ǘ�\"    = " + i�Ǘ��m�n +", \n"
// MOD 2010.11.10 ���s�j���� �X�V�ҁA�X�V�o�f�̍��ڂ̏C�� START
//						+        "�X�V�o�f                  = '" + sData[40] +"', \n"
//						+        "�X�V��                    = '" + sData[41] +"', \n"
						+        "�X�V�o�f                  = '" + sData[44] +"', \n"
						+        "�X�V��                    = '" + sData[45] +"', \n"
// MOD 2010.11.10 ���s�j���� �X�V�ҁA�X�V�o�f�̍��ڂ̏C�� END
						+        "�X�V����                  =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE ����b�c       = '" + sData[0] +"' \n"
						+ "   AND ����b�c       = '" + sData[1] +"' \n"
						+ "   AND �폜�e�f = '0'";

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
//					string s�o�^�� = "";
//					int i�Ǘ��m�n  = 0;
//					string s���t   = "";
//					string[] sData = sList[i].Split(',');
//					string cmdQuery = "";
//					string[] sRet2 = Get_JurnalNo(sUser, sData[0], sData[1], sData[40]);
//					if(sRet2[0].Length == 4){
//						s�o�^��   = sRet2[1];
//						s���t     = sRet2[1];
//						i�Ǘ��m�n = int.Parse(sRet2[2]);
//					}else{
//						tran.Rollback();
//						return sRet2;
//					}
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� END
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
					//�d���b�c�擾
					string s���X�b�c = sData[21];
					string s���X�b�c = sData[15];
					string s�d���b�c = " ";
					if(s���X�b�c.Trim().Length > 0 && s���X�b�c.Trim().Length > 0){
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
						string[] sRetSiwake = Get_siwake(sUser, conn2, s���X�b�c, s���X�b�c);
//						string[] sRetSiwake = new string[2]{""," "};
//						if(s���X�b�c��.Length > 0 && s���X�b�c��.Length > 0
//							&& s���X�b�c == s���X�b�c�� && s���X�b�c == s���X�b�c��)
//						{
//							sRetSiwake[1] = s�d���b�c��;
//						}
//						else
//						{
//							sRetSiwake = Get_siwake(sUser, conn2, s���X�b�c, s���X�b�c);
//							s���X�b�c�� = s���X�b�c;
//							s���X�b�c�� = s���X�b�c;
//							s�d���b�c�� = sRetSiwake[1];
//						}
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� END
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j START
//						sRet[0] = sRetSiwake[0];
// DEL 2007.03.10 ���s�j���� �d���b�c�̒ǉ��i�G���[�\����Q�Ή��j END
//						if(sRet[0] != " ") return sRet;
						s�d���b�c = sRetSiwake[1];
					}
// ADD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V START
					if(sData[4] != " ")
					{
						cmdQuery
							= "UPDATE �r�l�O�Q�׎�l \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V START
//							+ " SET ���O�R = TO_CHAR(SYSDATE,'YYYYMM') \n"
							+ " SET �o�^�o�f = TO_CHAR(SYSDATE,'YYYYMMDD') \n"
// MOD 2010.02.02 ���s�j���� �׎�l�}�X�^��[�o�^�o�f]�ɍŏI�g�p�����X�V END
							+ " WHERE ����b�c = '" + sData[0] +"' \n"
							+ " AND ����b�c   = '" + sData[1] +"' \n"
							+ " AND �׎�l�b�c = '" + sData[4] +"' \n"
							+ " AND �폜�e�f   = '0'";
						try{
							int iUpdRowSM02 = CmdUpdate(sUser, conn2, cmdQuery);
						}catch(Exception){
							;
						}
					}
// ADD 2009.01.30 ���s�j���� [���O�R]�ɍŏI���p�N�����X�V END

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//					// �����O�Q�ɍː�����яd�ʂ̎Q�l�l������
//					string s�ː� = "";
//					string s�d�� = "";
//					string s�ː��d�� = "";
//					try{
//						s�ː� = sData[27].Trim().PadLeft(5,'0');
//						s�d�� = sData[28].Trim().PadLeft(5,'0');
//						s�ː��d�� = s�ː�.Substring(0,5)
//									+ s�d��.Substring(0,5);
//					}catch(Exception){
//					}
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
//���ӁF���J�o���[���ł��Ȃ��Ȃ�
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//					sData[27] = "0"; // �ː�
//					sData[28] = "0"; // �d��
					if(s�d�ʓ��͐��� == "0"){
						sData[27] = "0"; // �ː�
						sData[28] = "0"; // �d��
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					string s�i���L���S = (sData.Length > 47) ? sData[47] : " ";
					string s�i���L���T = (sData.Length > 48) ? sData[48] : " ";
					string s�i���L���U = (sData.Length > 49) ? sData[49] : " ";
					if(s�i���L���S.Length == 0) s�i���L���S = " ";
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
					cmdQuery 
						= "INSERT INTO \"�r�s�O�P�o�׃W���[�i��\" \n"
// MOD 2010.10.13 ���s�j���� [�i���L���S]�ȂǍ��ڒǉ� START
						+ "(����b�c, ����b�c, �o�^��, \"�W���[�i���m�n\", �o�ד� \n"
						+ ", ���q�l�o�הԍ�, �׎�l�b�c \n"
						+ ", �d�b�ԍ��P, �d�b�ԍ��Q, �d�b�ԍ��R, �e�`�w�ԍ��P, �e�`�w�ԍ��Q, �e�`�w�ԍ��R \n"
						+ ", �Z���b�c, �Z���P, �Z���Q, �Z���R \n"
						+ ", ���O�P, ���O�Q, ���O�R, �X�֔ԍ� \n"
						+ ", ���X�b�c, ���X��, ����v \n"
						+ ", �ב��l�b�c, �ב��l������ \n"
						+ ", �W��X�b�c, ���X�b�c, ���X�� \n"
						+ ", ���Ӑ�b�c, ���ۂb�c, ���ۖ� \n"
						+ ", ��, �ː�, �d��, ���j�b�g \n"
						+ ", �w���, �w����敪 \n"
						+ ", �A���w���b�c�P, �A���w���P \n"
						+ ", �A���w���b�c�Q, �A���w���Q \n"
						+ ", �i���L���P, �i���L���Q, �i���L���R \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
						+ ", �i���L���S, �i���L���T, �i���L���U \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
						+ ", �����敪, �ی����z, �^��, ���p, ������ \n"
						+ ", �d���b�c, �����ԍ�, �����敪 \n"
						+ ", ����󔭍s�ςe�f, �o�׍ςe�f, ���M�ςe�f, �ꊇ�o�ׂe�f \n"
						+ ", ���, �ڍ׏�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
						+ ", �����O�Q \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�דo�^���́A�����O�S�Ɂu0�v��ݒ肷��
						+ ",�����O�S \n"
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
						+ ", �폜�e�f, �o�^����, �o�^�o�f, �o�^�� \n"
						+ ", �X�V����, �X�V�o�f, �X�V�� \n"
						+ ") \n"
// MOD 2010.10.13 ���s�j���� [�i���L���S]�ȂǍ��ڒǉ� END
						+ "VALUES ('" + sData[0]  +"','" + sData[1]  +"','" + s���t +"'," + i�Ǘ��m�n +",'" + sData[2] +"', \n"
						+         "'" + sData[3]  +"','" + sData[4]  +"', \n"
						+         "'" + sData[5]  +"','" + sData[6]  +"','" + sData[7] +"',' ',' ',' ', \n"
						+         "'" + sData[8]  +"','" + sData[9]  +"','" + sData[10] +"','" + sData[11] +"', \n"
						+         "'" + sData[12] +"','" + sData[13] +"',' ', '" + sData[14] +"', \n"
						+         "'" + sData[15] +"','" + sData[16] +"','" + sData[17] +"', \n"
						+         "'" + sData[18] +"','" + sData[19] +"', \n"
						+         "'" + sData[20] +"','" + sData[21] +"','" + sData[22] +"', \n"
						+         "'" + sData[23] +"','" + sData[24] +"','" + sData[25] +"', \n"
						+         ""  + sData[26] +","   + sData[27] +","   + sData[28] +",0, \n"
// MOD 2005.06.20 ���s�j�ɉ� ���C�A�E�g�ύX START
// MOD 2005.05.31 ���s�j�ɉ� �w����敪�ǉ� START
//						+         "'" + sData[29] +"',' ','" + sData[30] +"',' ','" + sData[31] +"', \n"
//						+         "'" + sData[29] +"','0','000','" + sData[30] +"','000','" + sData[31] +"', \n"
						+         "'" + sData[29] +"','" + sData[30] +"','" + sData[31] +"','" + sData[32] +"','" + sData[33] +"','" + sData[34] +"', \n"
// MOD 2005.05.31 ���s�j�ɉ� �w����敪�ǉ� END
// MOD 2005.06.20 ���s�j�ɉ� ���C�A�E�g�ύX END
						+         "'" + sData[35] +"','" + sData[36] +"','" + sData[37] +"', \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
						+         "'" + s�i���L���S +"','"+ s�i���L���T +"','"+ s�i���L���U +"', \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
						+         "'" + sData[38] +"',"  + sData[39] +",0,0,0, \n"						//�^���@���p�@������
// MOD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� START
//						+         "' ',' ',' ',"														//�d���b�c  �����ԍ�  �����敪
						+         "'" + s�d���b�c + "',' ',' ',"  //  �d���b�c  �����ԍ�  �����敪
// MOD 2007.02.08 ���s�j���� �d���b�c�̒ǉ� END
						+         "'" + sData[41] +"','" + sData[42] +"', '0', '" + sData[43] +"', \n"  //���M�ςe�f
						+         "'01','  ', \n"														//��ԁ@�ڍ׏��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//						+         "'" + s�ː��d�� + "', \n" // �����O�Q
						+         "' ', \n" // �����O�Q
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� START
						//�o�דo�^���́A�����O�S�Ɂu0�v��ݒ肷��
						+         "'0', \n" // �����O�S
// MOD 2016.06.24 bevas) ���{ �o�׏C���f�[�^���x�X�֐���ɔ��f�ł���悤�ɑΉ� END
						+         "'0',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[44] +"','" + sData[45] +"', \n"
						+             "TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + sData[44] +"','" + sData[45] +"')";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "�����o�דo�^["+sData[1]+"]["+s���t+"]["+i�Ǘ��m�n+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
					iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
//�ۗ� MOD 2007.04.27 ���s�j���� ORA-01000 �Ή� END
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� START
					// �Ǘ��ԍ���ێ�����
					sRet[iRetCnt++] = s���t.Trim();
					sRet[iRetCnt++] = i�Ǘ��m�n.ToString().Trim();
// MOD 2010.04.07 ���s�j���� �o�ׂb�r�u������� END
				}
				tran.Commit();
				logWriter(sUser, INF, "����I��");
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}
// ADD 2005.06.02 ���s�j�ɉ� ���e�m�F���������� START
		/*********************************************************************
		 * �����o�דo�^�p�Z���擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�ד�
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_autoEntryPref(string[] sUser, string sYcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�Z���擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
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
				cmdQuery = "SELECT CM14.�s���{���b�c,CM14.�s�撬���b�c,CM14.�厚�ʏ̂b�c \n"
					+      ",NVL(CM10.�X���b�c,' '),NVL(CM10.�X����,' ') \n"
					+      " FROM �b�l�P�S�X�֔ԍ� CM14 \n"
					+      " LEFT JOIN �b�l�P�O�X�� CM10 \n"
					+        "  ON CM14.�X���b�c = CM10.�X���b�c \n"
					+        " AND '0' = CM10.�폜�e�f \n"
					+      " WHERE CM14.�X�֔ԍ� = '" + sYcode + "' \n"
					+        " AND CM14.�폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim()	// �s���{���b�c
							+ reader.GetString(1).Trim()	// �s�撬���b�c
							+ reader.GetString(2).Trim();	// �厚�ʏ̂b�c
					sRet[2] = reader.GetString(3).Trim();	// �X���b�c
					sRet[3] = reader.GetString(4).Trim();	// �X����

					sRet[0] = "����I��";
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}

			return sRet;
		}

		/*********************************************************************
		 * �����o�דo�^�p�˗���擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�ד�
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_autoEntryClaim(string[] sUser, string sKcode, string sBcode, string sIcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�˗�����擾�J�n");

			OracleConnection conn2 = null;
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�̌Œ�d�ʁA�Œ�ː��̍l���ǉ� START
//			string[] sRet = new string[4];
			string[] sRet = new string[6];
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�̌Œ�d�ʁA�Œ�ː��̍l���ǉ� END

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			try
			{
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�擾���ڒǉ��i�d�ʁA�ː��j START
//// MOD 2010.07.30 ���s�j���� �����o�׎��̉ב��l���擾�̒��� START
////				string cmdQuery = "SELECT SM01.���Ӑ�b�c,SM01.���Ӑ敔�ۂb�c,SM04.���Ӑ敔�ۖ� \n"
//				string cmdQuery = "SELECT SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c, NVL(SM04.���Ӑ敔�ۖ�,' ') \n"
				string cmdQuery = "SELECT SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c, NVL(SM04.���Ӑ敔�ۖ�,' ') , SM01.�d��, SM01.�ː� \n"
//// MOD 2010.07.30 ���s�j���� �����o�׎��̉ב��l���擾�̒��� END
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�擾���ڒǉ��i�d�ʁA�ː��j END
					+ " FROM �r�l�O�P�ב��l SM01 \n"
					+ " LEFT JOIN �b�l�O�Q���� CM02 \n"
					+   " ON SM01.����b�c = CM02.����b�c \n"
					+  " AND SM01.����b�c = CM02.����b�c \n"
					+  " AND '0' = CM02.�폜�e�f \n"
					+ " LEFT JOIN �r�l�O�S������ SM04 \n"
					+   " ON CM02.����b�c = SM04.����b�c \n"
					+  " AND CM02.�X�֔ԍ� = SM04.�X�֔ԍ� \n"
					+  " AND SM01.���Ӑ�b�c = SM04.���Ӑ�b�c \n"
					+  " AND SM01.���Ӑ敔�ۂb�c = SM04.���Ӑ敔�ۂb�c \n"
					+  " AND '0' = SM04.�폜�e�f \n"
					+ " WHERE SM01.�ב��l�b�c = '" + sIcode + "' \n"
					+   " AND SM01.����b�c = '" + sKcode + "' \n"
					+   " AND SM01.����b�c = '" + sBcode + "' \n"
					+   " AND SM01.�폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[1]  = reader.GetString(0).Trim();
					sRet[2]  = reader.GetString(1).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[3]  = reader.GetString(2).Trim();
					sRet[3]  = reader.GetString(2).TrimEnd(); // ���Ӑ敔�ۖ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�擾���ڒǉ��i�d�ʁA�ː��j START
					sRet[4]  = reader.GetDecimal(3).ToString("#,##0").Trim(); // �d��
					sRet[5]  = reader.GetDecimal(4).ToString("#,##0").Trim(); // �ː�
// MOD 2016.02.02 BEVAS�j���{ �ב��l�}�X�^�擾���ڒǉ��i�d�ʁA�ː��j END

					sRet[0] = "����I��";
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.02 ���s�j�ɉ� ���e�m�F���������� END

// ADD 2005.06.10 ���s�j�����J �o�ד��X�V START
		/*********************************************************************
		 * �o�ד��X�V
		 * �����F����b�c�A����b�c�A�o�ד�...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Upd_syukkabi(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�ד��X�V�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[5];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
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
						= "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n"
						+    "SET �o�ד�             = '" + sData[4]  +"', \n"
						+        "�X�V�o�f           = '" + sData[5] +"',"
						+        "�X�V��             = '" + sData[6] +"', \n"
						+        "�X�V����           =  TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ " WHERE ����b�c           = '" + sData[0]  +"' \n"
						+ "   AND ����b�c           = '" + sData[1]  +"' \n"
						+ "   AND �o�^��             = '" + sData[2] +"' \n"
						+ "   AND \"�W���[�i���m�n\" = '" + sData[3] +"' \n"
//�ۗ��F�����̂o�f�ɂ͂ǂ��Ή����邩�H
//�ۗ� MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V START
//�ۗ�					+ "   AND �����ԍ�         = ' ' \n"
//�ۗ� MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V END
						+ "   AND �폜�e�f           = '0' \n";

					int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
					tran.Commit();
//�ۗ� MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V START
					if(iUpdRow == 0)
						sRet[0] = "�f�[�^�ҏW���ɑ��̒[�����X�V����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
					else
						sRet[0] = "����I��";
//					sRet[0] = "����I��";
//�ۗ� MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V END
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.06.10 ���s�j�����J �o�ד��X�V END

// ADD 2006.07.25 ���s�j�R�{ �o�׏Ɖ�̕\�����ڒǉ� START
		/*********************************************************************
		 * �o�׈ꗗ�擾
		 * �����F����b�c�A����b�c�A�׎�l�b�c�A�ב��l�b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I�����A���
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�o�ד��A�Z���P�A���O�P�A�j...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA1_SELECT_1 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " NVL(COUNT(S.ROWID),0), \n"
			+       " NVL(SUM(S.��),0), \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
////			+       " NVL(SUM(S.�d��),0), \n"
////			+       " NVL(SUM(S.�ː�),0) \n"
//			+       " NVL(SUM(DECODE(S.�^���d��,'     ',0,S.�^���d��)),0), \n"
//			+       " NVL(SUM(DECODE(S.�^���ː�,'     ',0,S.�^���ː�)),0), \n"
//			+       " 1 \n"
//// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
			+       " NVL(SUM(S.�d��),0) \n"
			+       ", NVL(SUM(S.�ː�),0) \n"
			+       ", NVL(SUM(DECODE(S.�^���d��,'     ',0,S.�^���d��)),0) \n"
			+       ", NVL(SUM(DECODE(S.�^���ː�,'     ',0,S.�^���ː�)),0) \n"
			+       ", NVL(MAX(CM01.�ۗ�����e�f),'0') \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			+  " FROM \"�r�s�O�P�o�׃W���[�i��\" S, �`�l�O�R��� J \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;

		private static string GET_SYUKKA1_SELECT_2 
			= "SELECT /*+ INDEX(S ST01IDX2) INDEX(J AM03PKEY) */ \n"
			+       " SUBSTR(S.�o�ד�,5,2) || '/' || SUBSTR(S.�o�ד�,7,2), S.�Z���P, S.���O�P, \n"
			+       " TO_CHAR(S.��), S.�d��, S.�A���w���P, \n"
			+       " S.�i���L���P, S.�����ԍ�, DECODE(S.�����敪,1,'����',2,'����',S.�����敪), \n"
			+       " DECODE(S.�w���,0,' ',(SUBSTR(S.�w���,5,2) || '/' || SUBSTR(S.�w���,7,2) || DECODE(S.�w����敪,'0','�K��','1','�w��',''))), \n"

			+       " DECODE(S.�ڍ׏��,'  ', NVL(J.��Ԗ�, S.���),NVL(J.��ԏڍז�, S.�ڍ׏��)), \n"
			+       " SUBSTR(S.�o�^��,5,2) || '/' || SUBSTR(S.�o�^��,7,2), \n"
			+       " S.���q�l�o�הԍ�, TO_CHAR(S.\"�W���[�i���m�n\"), S.�o�^��, \n"
			+       " SUBSTR(S.�o�ד�,1,4) || '/' || SUBSTR(S.�o�ד�,5,2) || '/' || SUBSTR(S.�o�ד�,7,2), \n"
			+       " S.�o�^��, \n"
			+       " S.�ː�, \n"
// MOD 2007.02.20 ���s�j���� �ی����̕\�� START
//			+       " S.�ی����z, \n"
			+       " S.������, \n"
// MOD 2007.02.20 ���s�j���� �ی����̕\�� END
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� START
//			+       " S.�^�� \n"
			+       " S.�^�� + S.���p \n"
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� END
// ADD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� START
			+       ", DECODE(S.�폜�e�f,'1','��',' ') \n"
			+       ", DECODE(S.����󔭍s�ςe�f,'1','��',' ') \n"
// ADD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� END
// ADD 2007.07.06 ���s�j���� �ꗗ���ڂɔ��X�b�c��\�� START
			+       ", S.���X�b�c \n"
// ADD 2007.07.06 ���s�j���� �ꗗ���ڂɔ��X�b�c��\�� END
// ADD 2008.10.29 ���s�j���� ���������ǉ� START
			+       ", S.���Ӑ�b�c \n"
			+       ", S.���ۂb�c \n"
			+       ", S.���ۖ� \n"
// ADD 2008.10.29 ���s�j���� ���������ǉ� END
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+       ", NVL(CM01.�L���A�g�e�f,'0') \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� START
			+       ", S.���, S.�o�׍ςe�f \n"
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			+       ", S.�^���ː�, S.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� START
			+       ", DECODE(S.�����O�R,'          ',' ',('20' || SUBSTR(S.�����O�R,1,2) || '/' || SUBSTR(S.�����O�R,3,2) || '/' || SUBSTR(S.�����O�R,5,2) || ' ' || SUBSTR(S.�����O�R,7,2) || ':' || SUBSTR(S.�����O�R,9,2))) \n"
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" S, �`�l�O�R��� J \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;

		private static string GET_SYUKKA1_SELECT_2_SORT
			= " ORDER BY �o�^��,\"�W���[�i���m�n\" ";

		private static string GET_SYUKKA1_SELECT_2_SORT2
			= " ORDER BY �o�ד�,�o�^��,\"�W���[�i���m�n\" ";

		[WebMethod]
		public String[] Get_syukka1(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai)
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
		{
			string[] sKey = new string[]{sKCode, sBCode, sTCode, sICode, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_syukka2(sUser, sKey);
		}
		[WebMethod]
		public String[] Get_syukka2(string[] sUser, string[] sKey)
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׈ꗗ�擾�Q�J�n");
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string sTCode   = sKey[2];
			string sICode   = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s�����ԍ��J�n = ""; if(sKey.Length >  8) s�����ԍ��J�n = sKey[ 8];
			string s�����ԍ��I�� = ""; if(sKey.Length >  9) s�����ԍ��I�� = sKey[ 9];
			string s���q�l�ԍ��J�n = ""; if(sKey.Length > 10) s���q�l�ԍ��J�n = sKey[10];
			string s���q�l�ԍ��I�� = ""; if(sKey.Length > 11) s���q�l�ԍ��I�� = sKey[11];
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� START
			string s�z���r�o�͌`�� = "";	if(sKey.Length > 12) s�z���r�o�͌`�� = sKey[12];
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� END

			if(s�����ԍ��J�n.Length == 0) s�����ԍ��J�n = s�����ԍ��I��;
			if(s�����ԍ��I��.Length == 0) s�����ԍ��I�� = s�����ԍ��J�n;
			if(s���q�l�ԍ��J�n.Length == 0) s���q�l�ԍ��J�n = s���q�l�ԍ��I��;
			if(s���q�l�ԍ��I��.Length == 0) s���q�l�ԍ��I�� = s���q�l�ԍ��J�n;
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END

			OracleConnection conn2 = null;
			string[] sRet = new string[4];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			string s�o�^���� = "0";
			string s�����v = "0";
			int    i�o�^���� = 0;
			decimal d�d�ʍ��v = 0;
			decimal d�ː����v = 0;
			string s�����    = "";
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː��d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
			decimal d�ː��d�ʍ��v = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			StringBuilder sbRet = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE S.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND S.����b�c = '" + sBCode + "' \n");
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� START
				if(s�����ԍ��J�n.Length > 0){
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX START
					if(s�����ԍ��J�n.Length >= 11){
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX END
						if(s�����ԍ��J�n == s�����ԍ��I��){
							sbQuery.Append(" AND S.�����ԍ� = '0000"+ s�����ԍ��J�n + "' \n");
						}else{
							sbQuery.Append(" AND S.�����ԍ� BETWEEN '0000"+ s�����ԍ��J�n
														 + "' AND '0000"+ s�����ԍ��I�� + "' \n");
						}
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX START
					}else if(s�����ԍ��J�n.Length >= 6){
						sbQuery.Append(" AND S.�����ԍ� BETWEEN '0000"+ s�����ԍ��J�n.PadRight(11,'0')
													 + "' AND '0000"+ s�����ԍ��I��.PadRight(11,'9') + "' \n");
					}else if(s�����ԍ��J�n.Length == 4
							|| s�����ԍ��J�n.Length == 5){
						if(s�����ԍ��J�n == s�����ԍ��I��){
							sbQuery.Append(" AND SUBSTR(S.�����ԍ�,"
							 +(5+11-s�����ԍ��J�n.Length)+","+s�����ԍ��J�n.Length
							 + ") = '" + s�����ԍ��J�n + "' \n");
						}else{
							sbQuery.Append(" AND SUBSTR(S.�����ԍ�,"
							 +(5+11-s�����ԍ��J�n.Length)+","+s�����ԍ��J�n.Length
							 + ") BETWEEN '" + s�����ԍ��J�n
							 + "' AND '"+ s�����ԍ��I�� + "' \n");
						}
					}else{
					}
// MOD 2011.03.17 ���s�j���� �����ԍ��̌����`�F�b�N�̕ύX END
				}
				if(s���q�l�ԍ��J�n.Length > 0){
					if(s���q�l�ԍ��J�n == s���q�l�ԍ��I��){
						sbQuery.Append(" AND S.���q�l�o�הԍ� = '"+ s���q�l�ԍ��J�n + "' \n");
					}else{
						sbQuery.Append(" AND S.���q�l�o�הԍ� BETWEEN '"+ s���q�l�ԍ��J�n
													 + "' AND '"+ s���q�l�ԍ��I�� + "' \n");
					}
				}
// MOD 2009.11.04 ���s�j���� ���������ɑ����ԍ��Ƃ��q�l�ԍ��̍��ڂ�ǉ� END

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND S.�׎�l�b�c = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND S.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND S.�׎�l�b�c = '"+ sTCode + "' \n");
					sbQuery.Append(" AND S.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(sSday != "0")
				{
					if(iSyuka == 0)
						sbQuery.Append(" AND S.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
					else
						sbQuery.Append(" AND S.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				}
				
				if(sJyoutai != "00")
				{
					if(sJyoutai == "aa")
						sbQuery.Append(" AND S.��� <> '01' \n");
					else
						sbQuery.Append(" AND S.��� = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND S.�폜�e�f = '0' \n");
				sbQuery.Append(" AND S.���     = J.��Ԃb�c(+) \n");
				sbQuery.Append(" AND S.�ڍ׏�� = J.��ԏڍׂb�c(+) \n");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
				sbQuery.Append(" AND S.����b�c = CM01.����b�c(+) \n");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END

				sbQuery2.Append(GET_SYUKKA1_SELECT_1);
				sbQuery2.Append(sbQuery);

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery2);

				if(reader.Read())
				{
					s�o�^����   = reader.GetDecimal(0).ToString("#,##0").Trim();
					s�����v   = reader.GetDecimal(1).ToString("#,##0").Trim();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					if(reader.GetString(6) == "1"){
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						d�d�ʍ��v   = reader.GetDecimal(2);
						d�ː����v   = reader.GetDecimal(3);
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}else{
						d�d�ʍ��v   = reader.GetDecimal(4);
						d�ː����v   = reader.GetDecimal(5);
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				sRet[1] = s�o�^����;
				sRet[2] = s�����v;
				d�d�ʍ��v = d�d�ʍ��v + d�ː����v * 8;
				sRet[3] = d�d�ʍ��v.ToString("#,##0").Trim();

				i�o�^���� = int.Parse(s�o�^����.Replace(",",""));

				if(i�o�^���� == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
				else
				{
					sRet = new string[i�o�^���� + 4];
					sRet[0] = "����I��";
					sRet[1] = s�o�^����;
					sRet[2] = s�����v;
					sRet[3] = d�d�ʍ��v.ToString("#,##0").Trim();

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

// ADD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� START
						sbRet.Append(sSepa + reader.GetString(20));			// �폜�e�f
						sbRet.Append(sSepa + reader.GetString(21));			// ����󔭍s�ςe�f
// ADD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� END
						sbRet.Append(sSepa + reader.GetString(0));			// �o�ד�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//						sbRet.Append(sSepa + reader.GetString(1).Trim());	// �Z���P
//						sbRet.Append(sCRLF + reader.GetString(2).Trim());	// ���O�P
						sbRet.Append(sSepa + reader.GetString(1).TrimEnd()); // �Z���P
						sbRet.Append(sCRLF + reader.GetString(2).TrimEnd()); // ���O�P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
						sbRet.Append(sSepa + reader.GetString(3));			// ��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//						d�ː����v = reader.GetDecimal(17);
//						d�ː����v = d�ː����v * 8;
//						if(d�ː����v == 0)
//							sbRet.Append(sSepa + reader.GetDecimal(4).ToString("#,##0").Trim()); // �d��
//						else
//							sbRet.Append(sSepa + d�ː����v.ToString("#,##0").Trim());		// �ː�
						s�^���ː� = reader.GetString(29).TrimEnd();
						s�^���d�� = reader.GetString(30).TrimEnd();
						d�ː��d�� = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
						s�d�ʓ��͐��� = reader.GetString(31).TrimEnd();
						if(s�d�ʓ��͐��� == "1" 
						&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//						&& (reader.GetDecimal(4) != 0 || reader.GetDecimal(17) != 0)
						){
							d�ː��d�� = reader.GetDecimal(17) * 8;	// �ː�
							d�ː��d�� += reader.GetDecimal(4);		// �d��
							sbRet.Append(sSepa + d�ː��d��.ToString("#,##0").Trim());
						}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
							if(s�^���ː�.Length > 0){
								try{
									d�ː��d�� += (Decimal.Parse(s�^���ː�) * 8);
								}catch(Exception){}
							}
							if(s�^���d��.Length > 0){
								try{
									d�ː��d�� += Decimal.Parse(s�^���d��);
								}catch(Exception){}
							}
							if(d�ː��d�� == 0){
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//�ۗ�								if(s�d�ʓ��͐��� == "1"){
//�ۗ�									sbRet.Append(sSepa + "0");
//�ۗ�								}else{
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
									sbRet.Append(sSepa + " ");
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//�ۗ�								}
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
							}else{
								sbRet.Append(sSepa + d�ː��d��.ToString("#,##0").Trim());
							}
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
						}
						d�ː��d�ʍ��v += d�ː��d��;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� START
//						sbRet.Append(sSepa + reader.GetDecimal(18).ToString("#,##0").Trim());
//																			// �ی���
//						sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
//																			// �^��
//
//						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());		// �A���w���P
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());		// �i���L���P
//						s����� = reader.GetString(7).Trim();              		// �����ԍ�
//						if(s�����.Length == 0)
//							sbRet.Append(sSepa + s�����);
//						else
//							sbRet.Append(sSepa + s�����.Remove(0,4));
//						sbRet.Append(sCRLF + reader.GetString(8));			// �����敪
//						sbRet.Append(sSepa + reader.GetString(9));			// �w���
//						sbRet.Append(sSepa + reader.GetString(10).Trim());	// ���
//						sbRet.Append(sSepa + reader.GetString(11));			// �o�^��
//						sbRet.Append(sSepa + reader.GetString(12).Trim());	// ���q�l�o�הԍ�
						s����� = reader.GetString(7).Trim();              		// �����ԍ�
						if(s�����.Length == 0)
							sbRet.Append(sSepa + s�����);
						else
							sbRet.Append(sSepa + s�����.Remove(0,4));
						sbRet.Append(sCRLF + reader.GetString(8));			// �����敪
// ADD 2007.07.06 ���s�j���� �ꗗ���ڂɔ��X�b�c��\�� START
						sbRet.Append("�@" + reader.GetString(22));			// ���X�b�c
// ADD 2007.07.06 ���s�j���� �ꗗ���ڂɔ��X�b�c��\�� END
						sbRet.Append(sSepa + reader.GetString(12).Trim());	// ���q�l�o�הԍ�
						sbRet.Append(sSepa + reader.GetString(9));			// �w���
						sbRet.Append(sSepa + reader.GetString(10).Trim());	// ���
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� START
						if(s�z���r�o�͌`��.Equals("1"))
						{
							sbRet.Append(sSepa + reader.GetString(32).Trim());// �z�����t�E����
						}
// MOD 2013.10.07 BEVAS�j���� �z�����t�E������ǉ� END
						sbRet.Append(sSepa + reader.GetString(5).TrimEnd());// �A���w���P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//						sbRet.Append(sCRLF + reader.GetString(6).Trim());	// �i���L���P
						sbRet.Append(sCRLF + reader.GetString(6).TrimEnd()); // �i���L���P
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
// MOD 2008.06.17 ���s�j���� �^�����O�̏ꍇ[��]�\�� START
//						sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
																			// �^��
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
//						if(reader.GetDecimal(19) == 0){
						if(reader.GetDecimal(19) == 0 || reader.GetString(26).Equals("1")){
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
							sbRet.Append(sSepa + "*");
						}else{
							sbRet.Append(sSepa + reader.GetDecimal(19).ToString("#,##0").Trim());
						}
// MOD 2008.06.17 ���s�j���� �^�����O�̏ꍇ[��]�\�� END
						sbRet.Append(sSepa + reader.GetDecimal(18).ToString("#,##0").Trim());
																			// �ی���
						sbRet.Append(sSepa + reader.GetString(11));			// �o�^��
// MOD 2007.01.17 ���s�j���� �ꗗ���ڂɍ폜�e�f�A����󔭍s�ςe�f��\�� END
						sbRet.Append(sSepa + reader.GetString(13));			// �W���[�i���m�n
						sbRet.Append(sSepa + reader.GetString(14));			// �o�^��
						sbRet.Append(sSepa + reader.GetString(15));			// �o�ד�
						sbRet.Append(sSepa + reader.GetString(16));			// �o�^��
// ADD 2008.10.29 ���s�j���� ���������ǉ� START
						sbRet.Append(sSepa + reader.GetString(23));			// ������b�c�i���Ӑ�b�c�j
						sbRet.Append(sSepa + reader.GetString(24));			// �����敔�ۂb�c
						sbRet.Append(sSepa + reader.GetString(25));			// �����敔�ۖ�
// ADD 2008.10.29 ���s�j���� ���������ǉ� END
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� START
						sbRet.Append(sSepa + reader.GetString(27));			// ���
						sbRet.Append(sSepa + reader.GetString(28));			// �o�׍ςe�f
// MOD 2010.11.12 ���s�j���� �����s�f�[�^���폜�\�ɂ��� END
						sbRet.Append(sSepa);
						sRet[iCnt] = sbRet.ToString();
						iCnt++;
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					sRet[3] = d�ː��d�ʍ��v.ToString("#,##0").Trim();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader);
					reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				}

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2006.07.25 ���s�j�R�{ �o�׏Ɖ�̕\�����ڒǉ� END
// ADD 2006.07.27 ���s�j�R�{ �o�׏Ɖ�̕\�����ڒǉ�(CSV�o��) START
		/*********************************************************************
		 * �o�׈ꗗ�擾�i�b�r�u�o�͗p�j
		 * �����F����b�c�A����b�c�A�׎�l�b�c�A�ב��l�b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I�����A���
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n�A�׎�l�b�c...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA1_SELECT_3
			= "SELECT /*+ INDEX(J ST01IDX2) INDEX(N SM01PKEY) */ \n"
			+       " J.�o�^��, J.�o�ד�, �����ԍ�, J.�׎�l�b�c, J.�X�֔ԍ�, \n"
			+       " '(' || TRIM(J.�d�b�ԍ��P) || ')' || TRIM(J.�d�b�ԍ��Q) || '-' || J.�d�b�ԍ��R, \n"
			+       " J.�Z���P, J.�Z���Q, J.�Z���R, J.���O�P, J.���O�Q, J.����v, J.���X�b�c, J.���X��, \n"
			+       " J.�ב��l�b�c, NVL(N.�X�֔ԍ�, ' '), \n"
			+       " NVL(N.�d�b�ԍ��P,' '), NVL(N.�d�b�ԍ��Q,' '), NVL(N.�d�b�ԍ��R,' '), \n"
			+       " NVL(N.�Z���P,' '), NVL(N.�Z���Q,' '), NVL(N.���O�P,' '), NVL(N.���O�Q,' '), \n"
			+       " TO_CHAR(J.��), TO_CHAR(J.�d��), \n"
			+       " J.�w���, J.�A���w���P, J.�A���w���Q, J.�i���L���P, J.�i���L���Q, J.�i���L���R, \n"
			+       " J.�����敪, TO_CHAR(J.�ی����z), J.���q�l�o�הԍ�, \n"
			+       " J.���Ӑ�b�c, J.���ۂb�c, J.�ː� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� START
//			+       " , TO_CHAR(J.�^��) \n"
			+       " , TO_CHAR(J.�^�� + J.���p) \n"
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� END
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+       " , NVL(CM01.�L���A�g�e�f,'0') \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
			+       " , J.�^���ː�, J.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J,�r�l�O�P�ב��l N \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;

		[WebMethod]
		public String[] Get_csvwrite1(string[] sUser, string sKCode, string sBCode, string sTCode, string sICode, 
										int iSyuka, string sSday, string sEday, string sJyoutai, string sIraiCd)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�b�r�u�o�͂P�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//			decimal d�ː� = 0;
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː��d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.����b�c = '" + sBCode + "' \n");

				if(sTCode.Length > 0 && sICode.Length == 0)
				{
					sbQuery.Append(" AND J.�׎�l�b�c = '"+ sTCode + "' \n");
				}
				if(sICode.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND J.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(sTCode.Length > 0 && sICode.Length > 0)
				{
					sbQuery.Append(" AND J.�׎�l�b�c = '"+ sTCode + "' \n");
					sbQuery.Append(" AND J.�ב��l�b�c = '"+ sICode + "' \n");
				}
				if(iSyuka == 0)
					sbQuery.Append(" AND J.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sIraiCd.Trim().Length != 0)
				{
					sbQuery.Append(" AND '" + sIraiCd + "' = J.�ב��l�b�c(+) \n");
				}

				if(sJyoutai != "00")
				{
// ADD 2008.07.09 ���s�j���� �����s�������O���� START
					if(sJyoutai == "aa")
						sbQuery.Append(" AND J.��� <> '01' \n");
					else
// ADD 2008.07.09 ���s�j���� �����s�������O���� END
						sbQuery.Append(" AND J.��� = '"+ sJyoutai + "' \n");
				}
				sbQuery.Append(" AND J.�폜�e�f = '0' \n");
				sbQuery.Append(" AND J.�ב��l�b�c     = N.�ב��l�b�c(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '0' = N.�폜�e�f(+) ");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
				sbQuery.Append(" AND J.����b�c = CM01.����b�c(+) \n");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END

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
					sbData.Append(sDbl + reader.GetString(0).Trim() + sDbl);					// �o�^��
					sbData.Append(sKanma + sDbl + reader.GetString(1).Trim() + sDbl       );	// �o�ד�
					string sNo = reader.GetString(2).Trim();									// �����ԍ�(XXX-XXXX-XXXX)
					if(sNo.Length == 15)
					{
						sbData.Append(sKanma + sDbl + sNo.Substring(4,3)
							+ "-" + sNo.Substring(7,4) + "-" + sNo.Substring(11) + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(3).Trim() + sDbl);	// �׎�l�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(4).Trim() + sDbl);	// �׎�l�X�֔ԍ�
					sbData.Append(sKanma + sDbl + reader.GetString(5).Trim() + sDbl);			// �׎�l�d�b�ԍ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sbData.Append(sKanma + sDbl + reader.GetString(6).Trim() + sDbl);			// �׎�l�Z���P
//					sbData.Append(sKanma + sDbl + reader.GetString(7).Trim() + sDbl);			// �׎�l�Z���Q
//					sbData.Append(sKanma + sDbl + reader.GetString(8).Trim() + sDbl);			// �׎�l�Z���R
//					sbData.Append(sKanma + sDbl + reader.GetString(9).Trim() + sDbl);			// �׎�l���O�P
//					sbData.Append(sKanma + sDbl + reader.GetString(10).Trim() + sDbl);			// �׎�l���O�Q
					sbData.Append(sKanma + sDbl + reader.GetString(6).TrimEnd() + sDbl);  // �׎�l�Z���P
					sbData.Append(sKanma + sDbl + reader.GetString(7).TrimEnd() + sDbl);  // �׎�l�Z���Q
					sbData.Append(sKanma + sDbl + reader.GetString(8).TrimEnd() + sDbl);  // �׎�l�Z���R
					sbData.Append(sKanma + sDbl + reader.GetString(9).TrimEnd() + sDbl);  // �׎�l���O�P
					sbData.Append(sKanma + sDbl + reader.GetString(10).TrimEnd() + sDbl); // �׎�l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(11).Trim() + sDbl);	// ����v
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(12).Trim() + sDbl);	// ���X�b�c
					sbData.Append(sKanma + sDbl + reader.GetString(13).Trim() + sDbl       );	// ���X��
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(14).Trim() + sDbl);	// �ב��l�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(15).Trim() + sDbl);	// �ב��l�X�֔ԍ�

					string sTel = reader.GetString(16).Trim();									// �ב��l�d�b�ԍ�
					if(sTel.Length != 0)
					{
						sbData.Append(sKanma + sDbl + "(" + sTel + ")"
							+ "-" + reader.GetString(17).Trim() + "-" + reader.GetString(18).Trim() + sDbl);
					}
					else
					{
						sbData.Append(sKanma + sDbl + " " + sDbl);
					}
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sbData.Append(sKanma + sDbl + reader.GetString(19).Trim() + sDbl);			// �ב��l�Z���P
//					sbData.Append(sKanma + sDbl + reader.GetString(20).Trim() + sDbl);			// �ב��l�Z���Q
//					sbData.Append(sKanma + sDbl + reader.GetString(21).Trim() + sDbl);			// �ב��l���O�P
//					sbData.Append(sKanma + sDbl + reader.GetString(22).Trim() + sDbl);			// �ב��l���O�Q
					sbData.Append(sKanma + sDbl + reader.GetString(19).TrimEnd() + sDbl); // �ב��l�Z���P
					sbData.Append(sKanma + sDbl + reader.GetString(20).TrimEnd() + sDbl); // �ב��l�Z���Q
					sbData.Append(sKanma + sDbl + reader.GetString(21).TrimEnd() + sDbl); // �ב��l���O�P
					sbData.Append(sKanma + sDbl + reader.GetString(22).TrimEnd() + sDbl); // �ב��l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sbData.Append(sKanma + reader.GetString(23)                            );	// ��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					d�ː� = reader.GetDecimal(36);													// �ː�
//					d�ː� = d�ː� * 8;
//					if(d�ː� == 0)
//						sbData.Append(sKanma + reader.GetString(24)                            );	// �d��
//					else
//						sbData.Append(sKanma + d�ː�.ToString()                            );
					s�^���ː� = reader.GetString(39).TrimEnd();
					s�^���d�� = reader.GetString(40).TrimEnd();
					d�ː��d�� = 0;
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(41).TrimEnd();
					if(s�d�ʓ��͐��� == "1"
					&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//					&& (reader.GetString(24).TrimEnd() != "0" || reader.GetDecimal(36) != 0)
					){
						d�ː��d�� += (reader.GetDecimal(36) * 8);		// �ː�
						if(reader.GetString(24).TrimEnd().Length > 0){	// �d��
							try{
								d�ː��d�� += Decimal.Parse(reader.GetString(24).TrimEnd());
							}catch(Exception){}
						}
						sbData.Append(sKanma + d�ː��d��.ToString());	// �d��
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						if(s�^���ː�.Length > 0){
							try{
								d�ː��d�� += (Decimal.Parse(s�^���ː�) * 8);
							}catch(Exception){}
						}
						if(s�^���d��.Length > 0){
							try{
								d�ː��d�� += Decimal.Parse(s�^���d��);
							}catch(Exception){}
						}
						sbData.Append(sKanma + d�ː��d��.ToString());		// �d��
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

					if(reader.GetString(25).Trim() == "0")
						sbData.Append(sKanma + sDbl + sDbl);										// �w���
					else
						sbData.Append(sKanma + sDbl + reader.GetString(25).Trim() + sDbl       );	// �w���
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(26).TrimEnd() + sDbl);	// �A���w���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(27).TrimEnd() + sDbl);	// �A���w���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(28).TrimEnd() + sDbl);	// �i���L���P
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(29).TrimEnd() + sDbl);	// �i���L���Q
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(30).TrimEnd() + sDbl);	// �i���L���R
					sbData.Append(sKanma + sDbl + reader.GetString(31).Trim() + sDbl       );	// �����敪
					sbData.Append(sKanma + reader.GetString(32)                            );	// �ی����z
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(33).Trim() + sDbl);	// ���q�l�o�הԍ�
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(34).Trim() + sDbl);	// ���Ӑ�b�c
					sbData.Append(sKanma + sDbl + sSng + reader.GetString(35).Trim() + sDbl);	// ���ۂb�c
// MOD 2008.06.17 ���s�j���� �^�����O�̏ꍇ[��]�\�� START
//					sbData.Append(sKanma + reader.GetString(37)                            );	// �^��
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
//					if(reader.GetString(37).Trim().Equals("0")){
					if(reader.GetString(37).Trim().Equals("0") || reader.GetString(38).Equals("1")){
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
						sbData.Append(sKanma + "*");
					}else{
						sbData.Append(sKanma + reader.GetString(37));
					}
// MOD 2008.06.17 ���s�j���� �^�����O�̏ꍇ[��]�\�� END
					sList.Add(sbData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}
// ADD 2006.07.27 ���s�j�R�{ �o�׏Ɖ�̕\�����ڒǉ�(CSV�o��) END
// ADD 2007.04.20 ���s�j���� �b�r�u�G���g���i�����o�דo�^�j�̍����� START
		/*********************************************************************
		 * �b�r�u�G���g���i�����o�דo�^�j�`�F�b�N�P
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�ד�
		 *
		 * ���Fsv_init.Get_syukabi(gsa���[�U,gs����b�c, gs����b�c);
		 * ���Fsv_syukka.Get_hatuten2(gsa���[�U,gs����b�c,gs����b�c);
		 * ���Fsv_syukka.Get_syuuyakuten2(gsa���[�U,gs����b�c,gs����b�c);
		 *
		 *********************************************************************/
		private static string CHECK_AUTOENTRY1_SELECT_1
			= "SELECT /*+ INDEX(CM02 CM02PKEY) INDEX(CM14 CM14PKEY) INDEX(CM10 CM10PKEY) */ \n"
			+ " CM02.�o�ד�, CM02.�X�֔ԍ�, CM14.�X���b�c, CM10.�X����, CM10.�W��X�b�c \n"
			+ " FROM �b�l�O�Q���� CM02, \n"
			+ " �b�l�P�S�X�֔ԍ� CM14, \n"
			+ " �b�l�P�O�X�� CM10 \n"
			;
		private static string CHECK_AUTOENTRY1_WHERE_1
			= " AND CM02.�폜�e�f = '0' \n"
			+ " AND CM02.�X�֔ԍ� = CM14.�X�֔ԍ� \n"
			+ " AND CM14.�X���b�c = CM10.�X���b�c \n"
			+ " AND CM10.�폜�e�f = '0' \n"
			;
		[WebMethod]
		public String[] Check_autoEntry1(string[] sUser, string sKCode, string sBCode)
		{
			logWriter(sUser, INF, "�b�r�u�G���g���i�����o�דo�^�j�`�F�b�N�P");

			OracleConnection conn2 = null;
			String[] sRet = new string[10];
			sRet[0] = "����I��";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(CHECK_AUTOENTRY1_SELECT_1);
				sbQuery.Append(" WHERE CM02.����b�c = '"+ sKCode + "' \n");
				sbQuery.Append(" AND CM02.����b�c = '"+ sBCode + "' \n");
				sbQuery.Append(CHECK_AUTOENTRY1_WHERE_1);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
					sRet[3] = reader.GetString(2).Trim();
					sRet[4] = reader.GetString(3).Trim();
					sRet[5] = reader.GetString(4).Trim();
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �͂�����擾�Q
		 * �����F����b�c�A����b�c�A�׎�l�b�c
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n�A�׎�l�b�c...
		 *
		 * ���Fsv_otodoke.Get_Stodokesaki(gsa���[�U,gs����b�c,gs����b�c,sKey[4]);
		 *
		 *********************************************************************/
		private static string GET_STODOKESAKI2_SELECT
			= "SELECT /*+ INDEX(SM02 SM02PKEY) */ \n"
			+ " SM02.�d�b�ԍ��P, SM02.�d�b�ԍ��Q, SM02.�d�b�ԍ��R \n"
			+ ", SM02.�Z���P, SM02.�Z���Q, SM02.�Z���R \n"
			+ ", SM02.���O�P, SM02.���O�Q, SM02.�X�֔ԍ�, SM02.����v \n"
			+ " FROM �r�l�O�Q�׎�l SM02 \n"
			;
		private static string GET_STODOKESAKI2_WHERE
			= " AND SM02.�폜�e�f = '0' \n"
			;

		[WebMethod]
		public String[] Get_Stodokesaki2(string[] sUser, string sKCode, string sBCode, string sTCode, string sYCode)
		{
			logWriter(sUser, INF, "�͂�����擾�Q�J�n");
			OracleConnection conn2 = null;
			String[] sRet = new string[15];
			sRet[0] = "����I��";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_STODOKESAKI2_SELECT);
				sbQuery.Append(" WHERE SM02.����b�c = '"+ sKCode + "' \n");
				sbQuery.Append(" AND SM02.����b�c = '"+ sBCode + "' \n");
				sbQuery.Append(" AND SM02.�׎�l�b�c = '" + sTCode + "' \n");
				sbQuery.Append(GET_STODOKESAKI2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();	//�d�b�ԍ��P
					sRet[2] = reader.GetString(1).Trim();	//�d�b�ԍ��Q
					sRet[3] = reader.GetString(2).Trim();	//�d�b�ԍ��R
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[4] = reader.GetString(3).Trim();	//�Z���P
//					sRet[5] = reader.GetString(4).Trim();	//�Z���Q
//					sRet[6] = reader.GetString(5).Trim();	//�Z���R
//					sRet[7] = reader.GetString(6).Trim();	//���O�P
//					sRet[8] = reader.GetString(7).Trim();	//���O�Q
					sRet[4] = reader.GetString(3).TrimEnd(); // �Z���P
					sRet[5] = reader.GetString(4).TrimEnd(); // �Z���Q
					sRet[6] = reader.GetString(5).TrimEnd(); // �Z���R
					sRet[7] = reader.GetString(6).TrimEnd(); // ���O�P
					sRet[8] = reader.GetString(7).TrimEnd(); // ���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sRet[9] = reader.GetString(8).Trim();	//�X�֔ԍ�
					sRet[10] = reader.GetString(9).Trim();	//����v

					//�X�֔ԍ��������͂ŁA���A�׎�l�̗X�֔ԍ������͂���Ă���ꍇ
					if(sYCode.Length == 0 && sRet[9].Length > 0){
						sbQuery = null;
						disposeReader(reader);
						reader = null;

						//�X�֔ԍ������i���X�̌���j
						sbQuery = new StringBuilder(1024);
						sbQuery.Append(GET_AUTOENTRYPREF2_SELECT);
						sbQuery.Append(" WHERE CM14.�X�֔ԍ� = '" + sRet[9] + "' \n");
						sbQuery.Append(GET_AUTOENTRYPREF2_WHERE);

						reader = CmdSelect(sUser, conn2, sbQuery);
						if (reader.Read()){
							sRet[11] = reader.GetString(0).Trim()	// �s���{���b�c
									+ reader.GetString(1).Trim()	// �s�撬���b�c
									+ reader.GetString(2).Trim();	// �厚�ʏ̂b�c
							sRet[12] = reader.GetString(3).Trim();	// �X���b�c
							sRet[13] = reader.GetString(4).Trim();	// �X����
						}else{
//							sRet[0] = "�Y���f�[�^������܂���";
							sRet[0] = "2";
						}
					}else{
						sRet[11] = "";
						sRet[12] = "";
						sRet[13] = "";
					}
				}else{
//					sRet[0] = "�Y���f�[�^������܂���";
					sRet[0] = "1";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "���͂�����擾�Q");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "�͂�����擾�Q");
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * ���X���擾�Q
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�Z���b�c�A�X���b�c�A�X����
		 *
		 * ���Fsv_syukka.Get_autoEntryPref(gsa���[�U,sKey[14]);
		 *
		 *********************************************************************/
		private static string GET_AUTOENTRYPREF2_SELECT
			= "SELECT /*+ INDEX(CM14 CM14PKEY) INDEX(CM10 CM10PKEY) */ \n"
			+ " CM14.�s���{���b�c, CM14.�s�撬���b�c, CM14.�厚�ʏ̂b�c \n"
			+ ", NVL(CM10.�X���b�c,' '), NVL(CM10.�X����,' ') \n"
			+ " FROM �b�l�P�S�X�֔ԍ� CM14 \n"
			+ ", �b�l�P�O�X�� CM10 \n"
			;
		private static string GET_AUTOENTRYPREF2_WHERE
			= " AND CM14.�X���b�c = CM10.�X���b�c(+) \n"
			+ " AND '0' = CM10.�폜�e�f(+) \n"
			;

		[WebMethod]
		public String[] Get_autoEntryPref2(string[] sUser, string sYCode)
		{
//			logWriter(sUser, INF, "���X���擾�Q�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];
			sRet[0] = "����I��";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_AUTOENTRYPREF2_SELECT);
				sbQuery.Append(" WHERE CM14.�X�֔ԍ� = '" + sYCode + "' \n");
				sbQuery.Append(GET_AUTOENTRYPREF2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim()	// �s���{���b�c
							+ reader.GetString(1).Trim()	// �s�撬���b�c
							+ reader.GetString(2).Trim();	// �厚�ʏ̂b�c
					sRet[2] = reader.GetString(3).Trim();	// �X���b�c
					sRet[3] = reader.GetString(4).Trim();	// �X����
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "�����X���擾�Q");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "���X���擾�Q");
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �˗�����擾�Q
		 * �����F����b�c�A����b�c�A����X�֔ԍ��A�ב��l�b�c
		 * �ߒl�F�X�e�[�^�X�A���Ӑ�b�c�A���Ӑ敔�ۂb�c�A���Ӑ敔�ۖ�
		 *
		 * ���Fsv_syukka.Get_autoEntryClaim(gsa���[�U,gs����b�c,gs����b�c,sKey[18]);
		 *
		 *********************************************************************/
		private static string GET_AUTOENTRYCLAIM2_SELECT
			= "SELECT /*+ INDEX(SM01 SM01PKEY) INDEX(SM04 SM04PKEY) */ \n"
			+ " SM01.���Ӑ�b�c, SM01.���Ӑ敔�ۂb�c, NVL(SM04.���Ӑ敔�ۖ�,' ') \n"
			+ " FROM �r�l�O�P�ב��l SM01 \n"
			+ ", �r�l�O�S������ SM04 \n"
			;
		private static string GET_AUTOENTRYCLAIM2_WHERE
			= " AND SM01.�폜�e�f = '0' \n"
			+ " AND SM01.���Ӑ�b�c = SM04.���Ӑ�b�c(+) \n"
			+ " AND SM01.���Ӑ敔�ۂb�c = SM04.���Ӑ敔�ۂb�c(+) \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
			+ " AND SM01.����b�c = SM04.����b�c(+) \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
			+ " AND '0' = SM04.�폜�e�f(+) \n"
			;

		[WebMethod]
		public String[] Get_autoEntryClaim2(string[] sUser, string sKCode, string sBCode
															, string sBYCode, string sICode)
		{
//			logWriter(sUser, INF, "�˗�����擾�Q�J�n");

			OracleConnection conn2 = null;
			String[] sRet = new string[5];
			sRet[0] = "����I��";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			StringBuilder sbQuery = null;
			OracleDataReader reader = null;
			try
			{
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_AUTOENTRYCLAIM2_SELECT);
				sbQuery.Append(" WHERE SM01.����b�c = '"+ sKCode + "' \n");
				sbQuery.Append(" AND SM01.����b�c = '"+ sBCode + "' \n");
				sbQuery.Append(" AND SM01.�ב��l�b�c = '" + sICode + "' \n");
				sbQuery.Append(" AND '" + sBYCode + "' = SM04.�X�֔ԍ�(+) \n");
				sbQuery.Append(GET_AUTOENTRYCLAIM2_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if (reader.Read()){
					sRet[1] = reader.GetString(0).Trim();	//���Ӑ�b�c
					sRet[2] = reader.GetString(1).Trim();	//���Ӑ敔�ۂb�c
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[3] = reader.GetString(2).Trim();	//���Ӑ敔�ۖ�
					sRet[3] = reader.GetString(2).TrimEnd(); // ���Ӑ敔�ۖ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}

//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "���˗�����擾�Q");
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "�˗�����擾�Q");
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
// ADD 2007.04.20 ���s�j���� �b�r�u�G���g���i�����o�דo�^�j�̍����� END
// ADD 2007.04.27 ���s�j���� ORA-01000 �Ή� START
		/*********************************************************************
		 * �W���[�i���m�n�擾
		 * �����F����b�c�A����b�c�A�X�V�o�f
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n
		 *
		 * ORA-01000: maximum open cursors exceeded �Ή�
		 *
		 *********************************************************************/
		private static string GET_JURNALNO_SELECT
			= "SELECT \"�W���[�i���m�n�o�^��\" \n"
			+ ", \"�W���[�i���m�n�Ǘ�\" \n"
			+ ", TO_CHAR(SYSDATE,'YYYYMMDD') \n"
			+ " FROM �b�l�O�Q���� \n"
			;
		private static string GET_JURNALNO_SELECT_WHERE
			= " AND �폜�e�f = '0' \n"
			+ " FOR UPDATE \n"
			;
		private static string GET_JURNALNO_UPDATE
			= "UPDATE �b�l�O�Q���� SET \n"
			+ " �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
			;
		private static string GET_JURNALNO_UPDATE_WHERE
			= " AND �폜�e�f = '0' \n"
			;
		private String[] Get_JurnalNo(string[] sUser, string sKCode, string sBCode, string sPGName)
		{
//			logWriter(sUser, INF, "�W���[�i���m�n�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[3];
			sRet[0] = "����I��";

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			string s�o�^�� = "";
			int i�Ǘ��m�n  = 0;
			string s���t   = "";

			OracleTransaction tran;
			tran = conn2.BeginTransaction();
			OracleDataReader reader = null;
			StringBuilder sbQuery;
			try
			{
				//�W���[�i���m�n�擾
				sbQuery = new StringBuilder(1024);
				sbQuery.Append(GET_JURNALNO_SELECT);
				sbQuery.Append(" WHERE ����b�c = '" + sKCode + "' \n");
				sbQuery.Append(" AND ����b�c = '" + sBCode + "' \n");
				sbQuery.Append(GET_JURNALNO_SELECT_WHERE);

				reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s�o�^��   = reader.GetString(0).Trim();
					i�Ǘ��m�n = reader.GetInt32(1);
					s���t     = reader.GetString(2).Trim();

					if(s�o�^�� == s���t)
					{
						i�Ǘ��m�n++;
					}
					else
					{
						s�o�^�� = s���t;
						i�Ǘ��m�n = 1;
					}
					if(i�Ǘ��m�n <= 9999)
					{
						sbQuery = new StringBuilder(1024);
						sbQuery.Append(GET_JURNALNO_UPDATE);
						sbQuery.Append(", \"�W���[�i���m�n�o�^��\" = '" + s�o�^�� + "' \n");
						sbQuery.Append(", \"�W���[�i���m�n�Ǘ�\" = " + i�Ǘ��m�n +" \n");
						sbQuery.Append(", �X�V�o�f = '" + sPGName +"' \n");
						sbQuery.Append(", �X�V��   = '" + sUser[1] +"' \n");
						sbQuery.Append(" WHERE ����b�c = '" + sKCode + "' \n");
						sbQuery.Append(" AND ����b�c = '" + sBCode + "' \n");
						sbQuery.Append(GET_JURNALNO_UPDATE_WHERE);

						int iUpdRow = CmdUpdate(sUser, conn2, sbQuery);
					}
					else
					{
						sRet[0] = "�W���[�i���m�n�̏���𒴂��܂���";
					}

					sRet[1] = s�o�^��;
					sRet[2] = i�Ǘ��m�n.ToString();
				}else{
					sRet[0] = "�Y���f�[�^������܂���";
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				tran.Commit();
//				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				logWriter(sUser, ERR, "���W���[�i���m�n�擾");
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				logWriter(sUser, ERR, "�W���[�i���m�n�擾");
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
// ADD 2007.04.27 ���s�j���� ORA-01000 �Ή� END

// ADD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX START
		/*********************************************************************
		 * ���X�擾
		 * �@�@�r�l�O�Q�׎�l�A�b�l�P�S�X�֔ԍ��A�b�l�P�T���X��\���A�b�l�P�X�X�֏Z��
		 *     �̂S�}�X�^���g�p���Ē��X�R�[�h�����肷��B
		 * �����F����R�[�h�A����R�[�h�A�׎�l�R�[�h�A�X�֔ԍ��A�Z���A����
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�Z���b�c
		 *
		 * Create : 2008.06.12 kcl)�X�{
		 * �@�@�@�@�@�@Get_tyakuten �����ɍ쐬
		 * Modify : 2008.12.24 kcl)�X�{
		 * �@�@�@�@�@�@�b�l�P�X�̌������@��ύX�A����ю�������̌�����ǉ�
		 *********************************************************************/
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@�̍ĕύX START
//		private String[] Get_tyakuten3(string[] sUser, OracleConnection conn2, 
//			string sKaiinCode, string sBumonCode, string sNiukeCode, 
//			string sYuubin, string sJuusyo)
		private String[] Get_tyakuten3(string[] sUser, OracleConnection conn2, 
			string sKaiinCode, string sBumonCode, string sNiukeCode, 
			string sYuubin, string sJuusyo, string sShimei)
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@�̍ĕύX END
		{
			string [] sRet = new string [4];		// �߂�l
			string cmdQuery;						// SQL��
			OracleDataReader reader;
			string tenCD       = string.Empty;		// �X���R�[�h
			string tenName     = string.Empty;		// �X����
			string juusyoCD    = string.Empty;		// �Z���R�[�h
			string address     = string.Empty;		// �Z��
			string niuJuusyoCD = string.Empty;		// �׎�l�}�X�^�̏Z���R�[�h
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			///
			/// ����P�i�K��
			/// �׎�l�}�X�^�̒��X�R�[�h������
			/// 
			string niuCode = sNiukeCode.Trim();
			if (niuCode.Length > 0) 
			{
				// SQL��
				cmdQuery
					= "SELECT SM02.����b�c, NVL(CM10.�X����, ' '), SM02.�Z���b�c \n"
					+ "  FROM �r�l�O�Q�׎�l SM02 \n"
					+ "  LEFT OUTER JOIN �b�l�P�O�X�� CM10 \n"
					+ "    ON SM02.����b�c   = CM10.�X���b�c \n"
					+ "   AND CM10.�폜�e�f   = '0' \n"
					+ " WHERE SM02.����b�c   = '" + sKaiinCode + "' \n"
					+ "   AND SM02.����b�c   = '" + sBumonCode + "' \n"
					+ "   AND SM02.�׎�l�b�c = '" + sNiukeCode + "' \n"
					+ "   AND ( LENGTH(TRIM(SM02.����b�c)) > 0 \n"
					+ "      OR LENGTH(TRIM(SM02.�Z���b�c)) > 0 ) \n"
					+ "   AND SM02.�폜�e�f   = '0' \n";

				// SQL���s
				// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
				//reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

				cmdQuery
					= "SELECT SM02.����b�c, NVL(CM10.�X����, ' '), SM02.�Z���b�c \n"
					+ "  FROM �r�l�O�Q�׎�l SM02 \n"
					+ "  LEFT OUTER JOIN �b�l�P�O�X�� CM10 \n"
					+ "    ON SM02.����b�c   = CM10.�X���b�c \n"
					+ "   AND CM10.�폜�e�f   = '0' \n"
					+ " WHERE SM02.����b�c   = :p_KaiinCD \n"
					+ "   AND SM02.����b�c   = :p_BumonCD \n"
					+ "   AND SM02.�׎�l�b�c = :p_NiukeCD \n"
					+ "   AND ( LENGTH(TRIM(SM02.����b�c)) > 0 \n"
					+ "      OR LENGTH(TRIM(SM02.�Z���b�c)) > 0 ) \n"
					+ "   AND SM02.�폜�e�f   = '0' \n";

				wk_opOraParam = new OracleParameter[3];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sKaiinCode, ParameterDirection.Input);
				wk_opOraParam[1] = new OracleParameter("p_BumonCD", OracleDbType.Char, sBumonCode, ParameterDirection.Input);
				wk_opOraParam[2] = new OracleParameter("p_NiukeCD", OracleDbType.Char, sNiukeCode, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

				// �f�[�^�擾
				if (reader.Read())
				{
					// �Y���f�[�^����

					// �f�[�^�擾
					tenCD    = reader.GetString(0).Trim();		// �X���R�[�h
					tenName  = reader.GetString(1).Trim();		// �X����
					juusyoCD = reader.GetString(2).Trim();		// �Z���R�[�h

					if (tenCD.Length > 0) 
					{
						// �׎�l�}�X�^�̒��X�R�[�h�����͂���Ă���ꍇ

						// �Z���R�[�h�̐ݒ�
						if (juusyoCD.Length == 0) 
						{
							// �׎�l�}�X�^�̏Z���R�[�h���󗓂̏ꍇ

							// �X�֔ԍ��}�X�^����擾
							string [] sResult = this.Get_juusyoCode(sUser, conn2, sYuubin);
							if (sResult[0] == " ") 
								juusyoCD = sResult[1];
						}

						// �߂�l���Z�b�g
						sRet[0] = " ";
						sRet[1] = tenCD;
						sRet[2] = tenName;
						sRet[3] = juusyoCD;

						// �I������
						disposeReader(reader);
						reader = null;
					
						return sRet;
					} 
					else
					{
						// �׎�l�}�X�^�ɏZ���R�[�h�݂̂����͂���Ă���ꍇ

						// �׎�l�}�X�^�̏Z���R�[�h���Ƃ��Ă���
						niuJuusyoCD = juusyoCD;
					}
				}

				// �I������
				disposeReader(reader);
				reader = null;
			}

			///
			/// ����Q�i�K��
			/// �X�֔ԍ��}�X�^���璅�X�R�[�h������
			///
// ADD 2008.10.31 ���s�j���� ���X�R�[�h�������ɒ��X��\���`�F�b�N��ǉ� START
			cmdQuery
				= "SELECT CM15.�X�֔ԍ� \n"
				+ " FROM �b�l�P�T���X��\�� CM15 \n"
				+ " WHERE CM15.�X�֔ԍ� = '" + sYuubin + "' \n"
				+ "   AND CM15.�폜�e�f = '0' \n";

			// SQL���s
			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery
				= "SELECT CM15.�X�֔ԍ� \n"
				+ " FROM �b�l�P�T���X��\�� CM15 \n"
				+ " WHERE CM15.�X�֔ԍ� = :p_YuubinNo \n"
				+ "   AND CM15.�폜�e�f = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// �f�[�^�擾
			if (reader.Read())
			{
				; // �X�֔ԍ��}�X�^�͌������Ȃ�
			}
			else
			{
				// �I������
				disposeReader(reader);
				reader = null;
// ADD 2008.10.31 ���s�j���� ���X�R�[�h�������ɒ��X��\���`�F�b�N��ǉ� END
				// SQL��
				cmdQuery
					= "SELECT CM14.�X���b�c, CM10.�X����, CM14.�s���{���b�c || CM14.�s�撬���b�c || CM14.�厚�ʏ̂b�c \n"
					+ "  FROM �b�l�P�S�X�֔ԍ� CM14 \n"
					+ " INNER JOIN �b�l�P�O�X�� CM10 \n"
					+ "    ON CM14.�X���b�c = CM10.�X���b�c \n"
					+ "   AND CM10.�폜�e�f = '0' \n"
					+ " WHERE CM14.�X�֔ԍ� = '" + sYuubin + "' \n"
					+ "   AND LENGTH(TRIM(CM14.�X���b�c)) > 0 \n"
					+ "   AND CM14.�폜�e�f = '0' \n";

				// SQL���s
				// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
				//reader = CmdSelect(sUser, conn2, cmdQuery);
				logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

				cmdQuery
					= "SELECT CM14.�X���b�c, CM10.�X����, CM14.�s���{���b�c || CM14.�s�撬���b�c || CM14.�厚�ʏ̂b�c \n"
					+ "  FROM �b�l�P�S�X�֔ԍ� CM14 \n"
					+ " INNER JOIN �b�l�P�O�X�� CM10 \n"
					+ "    ON CM14.�X���b�c = CM10.�X���b�c \n"
					+ "   AND CM10.�폜�e�f = '0' \n"
					+ " WHERE CM14.�X�֔ԍ� = :p_YuubinNo \n"
					+ "   AND LENGTH(TRIM(CM14.�X���b�c)) > 0 \n"
					+ "   AND CM14.�폜�e�f = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;
				// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

				// �f�[�^�擾
				if (reader.Read())
				{
					// �Y���f�[�^����

					// �f�[�^�擾
					tenCD    = reader.GetString(0).Trim();		// �X���R�[�h
					tenName  = reader.GetString(1).Trim();		// �X����
					juusyoCD = reader.GetString(2).Trim();		// �Z���R�[�h

					// �߂�l���Z�b�g
					sRet[0] = " ";
					sRet[1] = tenCD;
					sRet[2] = tenName;
					sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
					// ���� �׎�l�}�X�^�̏Z���R�[�h��D�悷��

					// �I������
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX START
				else 
				{
					// �b�l�P�S�ɊY���f�[�^�Ȃ�

					// �߂�l���Z�b�g
					sRet[0] = "���͂��ꂽ���͂���(�X�֔ԍ�)�ł͔z�B�X�����߂��܂���ł���";
					sRet[1] = "0000";
					sRet[2] = " ";
					sRet[3] = " ";

					// �I������
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX END
// ADD 2008.10.31 ���s�j���� ���X�R�[�h�������ɒ��X��\���`�F�b�N��ǉ� START
			}
// ADD 2008.10.31 ���s�j���� ���X�R�[�h�������ɒ��X��\���`�F�b�N��ǉ� END
			// �I������
			disposeReader(reader);
			reader = null;

			///
			/// ����R�i�K��
			/// �X�֏Z���}�X�^���璅�X�R�[�h������
			/// 
			// SQL��
			cmdQuery
				= "SELECT CM19.�X���b�c, CM10.�X����, CM19.�Z���b�c, CM19.�Z�� \n"
				+ "  FROM �b�l�P�X�X�֏Z�� CM19 \n"
				+ " INNER JOIN �b�l�P�O�X�� CM10 \n"
				+ "    ON CM19.�X���b�c = CM10.�X���b�c \n"
				+ "   AND CM10.�폜�e�f = '0' \n"
				+ " WHERE CM19.�X�֔ԍ� = '" + sYuubin + "' \n"
				+ "   AND CM19.�폜�e�f = '0' \n"
// ADD 2009.01.16 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX START
				+ " ORDER BY "
				+ "       LENGTH(TRIM(CM19.�Z��)) DESC \n"
// ADD 2009.01.16 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX END
				;

			// SQL���s
			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery
				= "SELECT CM19.�X���b�c, CM10.�X����, CM19.�Z���b�c, CM19.�Z�� \n"
				+ "  FROM �b�l�P�X�X�֏Z�� CM19 \n"
				+ " INNER JOIN �b�l�P�O�X�� CM10 \n"
				+ "    ON CM19.�X���b�c = CM10.�X���b�c \n"
				+ "   AND CM10.�폜�e�f = '0' \n"
				+ " WHERE CM19.�X�֔ԍ� = :p_YuubinNo \n"
				+ "   AND CM19.�폜�e�f = '0' \n"
				+ " ORDER BY "
				+ "       LENGTH(TRIM(CM19.�Z��)) DESC \n"
				;
			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// �f�[�^�擾
// MOD 2008.12.24 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX START
//			int dst;					// �ގ��x
//			int minDst = 999999;		// �ŏ��̗ގ��x�i�ł����Ă��j
//			int len;					// ��������Z���̒���
//			int maxLen = 0;				// �ŏ��ގ��x�̏Z���̍ő咷��
//			while (reader.Read())
//			{
//				// �Z���̎擾
//				address = reader.GetString(3).Trim();
//				len = address.Length;
//
//				// �ގ��x�̎Z�o
//				dst = this.GetSED(sJuusyo, address);
//
//				if (dst < minDst) 
//				{
//					// ���܂ł̂�莗�Ă�
//
//					// �f�[�^�擾
//					tenCD    = reader.GetString(0).Trim();		// �X���R�[�h
//					tenName  = reader.GetString(1).Trim();		// �X����
//					juusyoCD = reader.GetString(2).Trim();		// �Z���R�[�h
//					minDst   = dst;
//					maxLen   = len;
//				} 
//				else if (dst == minDst) 
//				{
//					// ���܂ł̂Ɠ������炢���Ă�
//
//					// ���������Z���̒������`�F�b�N
//					if (len > maxLen) 
//					{
//						// ���������Z�������܂ł�蒷��
//
//						// �f�[�^�擾
//						tenCD    = reader.GetString(0).Trim();	// �X���R�[�h
//						tenName  = reader.GetString(1).Trim();	// �X����
//						juusyoCD = reader.GetString(2).Trim();	// �Z���R�[�h
//						maxLen   = len;
//					}
//				}
//			}
//			if (tenCD.Length > 0) 
//			{
//				// �Y���f�[�^����
//
//				// �߂�l���Z�b�g
//				sRet[0] = " ";
//				sRet[1] = tenCD;
//				sRet[2] = tenName;
//				sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
//				// ���� �׎�l�}�X�^�̏Z���R�[�h��D�悷��
//
//				// �I������
//				disposeReader(reader);
//				reader = null;
//		
//				return sRet;
//			}
			while (reader.Read()) 
			{
				// �Z���̎擾
				address = reader.GetString(3).Trim();

// ADD 2009.02.20 kcl)�X�{ �b��Ή� START
				if (sShimei == null) sShimei = " ";
// ADD 2009.02.20 kcl)�X�{ �b��Ή� END

				// �Z���E�����̃`�F�b�N
				if ((sJuusyo.IndexOf(address) >= 0) ||
					(sShimei.IndexOf(address) >= 0))
				{
					// �f�[�^�擾
					tenCD    = reader.GetString(0).Trim();	// �X���R�[�h
					tenName  = reader.GetString(1).Trim();	// �X����
					juusyoCD = reader.GetString(2).Trim();	// �Z���R�[�h

					// �߂�l���Z�b�g
					sRet[0] = " ";
					sRet[1] = tenCD;
					sRet[2] = tenName;
					sRet[3] = (niuJuusyoCD.Length > 0) ? niuJuusyoCD : juusyoCD;
					// ���� �׎�l�}�X�^�̏Z���R�[�h��D�悷��

					// �I������
					disposeReader(reader);
					reader = null;
			
					return sRet;
				}
			}
// MOD 2008.12.24 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX END

			// �I������
			disposeReader(reader);
			reader = null;

			// �Y���f�[�^��
// MOD 2008.11.19 ���s�j���� ���X�R�[�h���󔒂ł��G���[�łȂ����� START
//			sRet[0] = "���͂��ꂽ���͂���(�X�֔ԍ�)�ł͔z�B�X�����߂��܂���ł���";
//			sRet[1] = "0000";
			sRet[0] = " ";
			sRet[1] = " ";
// MOD 2008.11.19 ���s�j���� ���X�R�[�h���󔒂ł��G���[�łȂ����� END
			sRet[2] = " ";
			sRet[3] = " ";
			
			return sRet;
		}

		/*********************************************************************
		 * �Z���R�[�h�擾
		 * �@�@�b�l�P�S�X�֔ԍ����g�p���āA�X�֔ԍ�����Z���R�[�h���擾����B
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�Z���b�c
		 *
		 * Create : 2008.06.16 kcl)�X�{
		 * �@�@�@�@�@�@�V�K�쐬
		 * Modify : 
		 *********************************************************************/
		private String[] Get_juusyoCode(string[] sUser, OracleConnection conn2, 
			string sYuubin)
		{
			string [] sRet = new string [2];	// �߂�l
			string cmdQuery;					// SQL��
			OracleDataReader reader;
			// ADD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			OracleParameter[]	wk_opOraParam	= null;
			// ADD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// SQL��
			cmdQuery
				= "SELECT CM14.�s���{���b�c || CM14.�s�撬���b�c || CM14.�厚�ʏ̂b�c \n"
				+ "  FROM �b�l�P�S�X�֔ԍ� CM14 \n"
				+ " WHERE CM14.�X�֔ԍ� = '" + sYuubin + "' \n"
				+ "   AND CM14.�폜�e�f = '0' \n";

			// SQL���s
			// MOD-S 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j
			//reader = CmdSelect(sUser, conn2, cmdQuery);
			logWriter(sUser, INF_SQL, "###�o�C���h��i�z��j###\n" + cmdQuery);	//�C���O��UPDATE�������O�o��

			cmdQuery
				= "SELECT CM14.�s���{���b�c || CM14.�s�撬���b�c || CM14.�厚�ʏ̂b�c \n"
				+ "  FROM �b�l�P�S�X�֔ԍ� CM14 \n"
				+ " WHERE CM14.�X�֔ԍ� = :p_YuubinNo \n"
				+ "   AND CM14.�폜�e�f = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_YuubinNo", OracleDbType.Char, sYuubin, ParameterDirection.Input);

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;
			// MOD-E 2012.09.06 COA)���R Oracle�T�[�o���׌y���΍�iSQL�Ƀo�C���h�ϐ��𗘗p�j

			// �f�[�^�擾
			if (reader.Read())
			{
				// �Y���f�[�^����
				sRet[0] = " ";							// �X�e�[�^�X
				sRet[1] = reader.GetString(0).Trim();	// �Z���R�[�h
			} 
			else
			{
				// �Y���f�[�^��
				sRet[0] = "���͂��ꂽ�X�֔ԍ��ł͏Z���R�[�h�����߂��܂���ł���";
				sRet[1] = " ";
			}

			// �I������
			disposeReader(reader);
			reader = null;
			
			return sRet;
		}

		/*********************************************************************
		 * SED(Shortest Edit Distance)�̎擾
		 * �@�@�Q�̕�����̗ގ��x�i�ŏ��G�f�B�b�g�����ASED�j���擾���܂��B
		 * �@�@�߂�l���������قǂ悭���Ă��܂��B
		 * �����F�������̕�����A�������镶����
		 * �ߒl�F�Q�̕������SED
		 *
		 * Create : 2008.06.12 kcl)�X�{
		 * �@�@�@�@�@�@�V�K�쐬
		 * Modify : 
		 *********************************************************************/
		private int GetSED(string srcStr, string fndStr) 
		{
			int i, j;
			int srcLen = 0;
			int fndLen = 0;
			int minDst = 999999;

			// Unicode ������𕶎����Ɉ������߂̏���
			TextElementEnumerator srcTee = StringInfo.GetTextElementEnumerator(srcStr);
			TextElementEnumerator fndTee = StringInfo.GetTextElementEnumerator(fndStr);

			// �e������̕��������v�Z
			while (srcTee.MoveNext()) 
				srcLen++;
			while (fndTee.MoveNext())
				fndLen++;
			srcTee.Reset();
			fndTee.Reset();

			// ������Ԃ̋������Z�o���邽�߂̔z��̏�����
			int [,] C = new int[fndLen+1, srcLen+1];
			for (i=0; i<=fndLen; i++) 
				C[i, 0] = i;
			for (j=0; j<=srcLen; j++) 
				C[0, j] = 0;

			// ������Ԃ̋������Z�o
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

			// ������Ԃ̍ŏ��������擾
			for (j=0; j<=srcLen; j++) 
				minDst = Math.Min(C[fndLen, j], minDst);

			return minDst;
		}

		/*********************************************************************
		 * �����o�דo�^�p�Z���擾�R
		 * �@�@�r�l�O�Q�׎�l�A�b�l�P�S�X�֔ԍ��A�b�l�P�T���X��\���A�b�l�P�X�X�֏Z��
		 *     �̂R�}�X�^���g�p���Ē��X�R�[�h�����肷��B
		 * �����F����R�[�h�A����R�[�h�A�׎�l�R�[�h�A�X�֔ԍ��A�Z���A����
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�Z���b�c
		 *
		 * Create : 2008.06.12 kcl)�X�{
		 * �@�@�@�@�@�@Get_autoEntryPref �����ɍ쐬
		 * Modify : 2008.12.25 kcl)�X�{
		 *            �����Ɏ�����ǉ�
		 *********************************************************************/
		[WebMethod]
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX START
//		public string [] Get_autoEntryPref3(string [] sUser, 
//			string sKaiinCode, string sBumonCode, string sNiukeCode, 
//			string sYuubin, string sJuusyo)
		public string [] Get_autoEntryPref3(string [] sUser, 
			string sKaiinCode, string sBumonCode, string sNiukeCode, 
			string sYuubin, string sJuusyo, string sShimei)
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX END
		{
			// ���O�o��
			logWriter(sUser, INF, "�Z���擾�R�J�n");

			OracleConnection conn2 = null;
			string [] sRet = new string [4];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// �c�a�ڑ��Ɏ��s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			try {
				// ���X�R�[�h�̎擾
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX START
//				string [] sResult = this.Get_tyakuten3(sUser, conn2, sKaiinCode, sBumonCode, sNiukeCode, sYuubin, sJuusyo);
				string [] sResult = this.Get_tyakuten3(sUser, conn2, sKaiinCode, sBumonCode, sNiukeCode, sYuubin, sJuusyo, sShimei);
// MOD 2008.12.25 kcl)�X�{ ���X�R�[�h�̌������@���ĕύX END

				if (sResult[0] == " ")
				{
					// �擾����
					sRet[1] = sResult[3];	// �Z���b�c
					sRet[2] = sResult[1];	// �X���b�c
					sRet[3] = sResult[2];	// �X����

					sRet[0] = "����I��";
				}
				else
				{
					// �擾���s
					sRet[0] = "�Y���f�[�^������܂���";
				}

				// ���O�o��
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// ����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX END

// ADD 2015.07.14 bevas)���{ �o�[�R�[�h�ǎ��p��ʂ̒ǉ� START
		/*********************************************************************
		 * �O���[�o���W�דX�����擾
		 * �@�@�b�l�Q�O�����X�}�X�^���g�p���āA�W�דX�̌������擾����B
		 * �����F����R�[�h
		 * �ߒl�F�X�e�[�^�X�A�W�דX����
		 *********************************************************************/
		[WebMethod]
		public string[] Get_GlobalCount(string[] sUser)
		{
			// ���O�o��
			logWriter(sUser, INF, "�O���[�o���W�דX�����擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]; // �߂�l
			string cmdQuery; // SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// �c�a�ڑ��Ɏ��s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			try 
			{
				// SQL��
				cmdQuery
					= "SELECT COUNT(*) \n"
					+ "  FROM �b�l�O�T������X \n"
					+ " WHERE ����b�c = :p_KaiinCD \n"
					+ "   AND �폜�e�f = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_KaiinCD", OracleDbType.Char, sUser[0], ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// �f�[�^�擾
				if (reader.Read())
				{
					// �Y���f�[�^����
					sRet[0] = "����I��";						// �X�e�[�^�X
					sRet[1] = reader.GetDecimal(0).ToString();	// �W�דX����
				} 
				else
				{
					// �Y���f�[�^�Ȃ�
					sRet[0] = "�Y������f�[�^�͑��݂��܂���B";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// ����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * �C���\�o�׃f�[�^�����擾
		 *		�r�s�O�P�o�׃W���[�i������A
		 *		�o�ד������ȍ~���C���\�ȏo�׃f�[�^�̌������擾����B
		 * ���� : ���[�ԍ�
		 * �ߒl : �X�e�[�^�X�A�o�׃f�[�^����
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ModifiableSyukkaCount(string[] sUser, string sInvoiceNo)
		{
			// ���O�o��
			logWriter(sUser, INF, "�C���\�o�׃f�[�^�����擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]; // �߂�l
			string cmdQuery; // SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				// �c�a�ڑ��Ɏ��s
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			//�����ԍ����e�[�u���i�[�`���ɕϊ�
			string sKey_InvoiceNo = string.Empty;
			sKey_InvoiceNo = "0000" + sInvoiceNo;

			try 
			{
				// SQL��
				cmdQuery
					= "SELECT COUNT(*) \n"
					+ "  FROM �r�s�O�P�o�׃W���[�i�� \n"
					+ " WHERE ���X�b�c   = '047' \n"
					+ "   AND �����ԍ� = :p_InvoiceNo \n"
					+ "   AND �폜�e�f   = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_InvoiceNo", OracleDbType.Char, sKey_InvoiceNo, ParameterDirection.Input);

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// �f�[�^�擾
				if (reader.Read())
				{
					// �Y���f�[�^����
					sRet[0] = "����I��";						// �X�e�[�^�X
					sRet[1] = reader.GetDecimal(0).ToString();	// �o�׃f�[�^����
				} 
				else
				{
					// �Y���f�[�^�Ȃ�
					sRet[0] = "�Y������f�[�^�͑��݂��܂���B";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				// ����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}
// ADD 2015.07.14 bevas)���{ �o�[�R�[�h�ǎ��p��ʂ̒ǉ� END
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� START
		/*********************************************************************
		 * �b�l�P�O���݃`�F�b�N�i�x�X�~�ߗp�j
		 * �@�@���͂��ꂽ�X�֔ԍ��ƏZ���R����A
		 * �@�@�b�l�P�O�X���̑��݃`�F�b�N�����{����B
		 * �����F�Z���R�i�X���R�[�h���i�[�j�A�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�X��������
		 *********************************************************************/
		[WebMethod]
		public string[] CheckCM10_GeneralDelivery(string[] sUser, string sJusyo3, string sYubinNo)
		{
			logWriter(sUser, INF, "�b�l�P�O���݃`�F�b�N�i�x�X�~�ߗp�j�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];	// �߂�l
			string cmdQuery;				// SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// �Z���R�ɐݒ肳�ꂽ�X���R�[�h�i�S�p�����j�𔼊p�����ϊ�
			string sTyakutenCode = this.���p�����ϊ�(sJusyo3);
			if(sTyakutenCode.Length != 3)
			{
				// ���p�ϊ����s
				sRet[0] = sTyakutenCode;
				sRet[1] = " ";
				return sRet;
			}

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				// SQL��
				cmdQuery
					= "SELECT �X�������� \n"
					+ "  FROM �b�l�P�O�X�� \n"
					+ " WHERE �X���b�c = :p_BranchCode \n"
					+ "   AND �X�֔ԍ� = :p_YubinNo \n"
					+ "   AND �폜�e�f = '0' \n";
				if(sUser[0].Substring(0,1) != "J")
				{
					cmdQuery += "   AND �x�X�~�߂e�f�P = '1' \n"; // �x�X�~�߂e�f�P�i���R�ʉ^�j

				}
				else
				{
					cmdQuery += "   AND �x�X�~�߂e�f�Q = '1' \n"; // �x�X�~�߂e�f�P�i���q�^���j
				}

				wk_opOraParam = new OracleParameter[2];
				wk_opOraParam[0] = new OracleParameter("p_BranchCode", OracleDbType.Char, sTyakutenCode, ParameterDirection.Input); // �X���b�c
				wk_opOraParam[1] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // �X�֔ԍ�

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// �f�[�^�擾
				if (reader.Read())
				{
					// �Y���f�[�^����
					sRet[0] = "����I��";								// �X�e�[�^�X
					sRet[1] = reader.GetString(0).ToString().Trim();	// �X��������
				} 
				else
				{
					// �Y���f�[�^�Ȃ�
					sRet[0] = "�X���}�X�^���݃`�F�b�N�F�Y������X���f�[�^�͑��݂��܂���B";
					sRet[1] = " ";
				}
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = " ";
			}
			catch (Exception ex)
			{
				// ����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				sRet[1] = " ";
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return sRet;
		}

		/*********************************************************************
		 * ���X�擾�i�x�X�~�ߗp�j
		 * �@�@�b�l�P�O�X���}�X�^���g�p���āA���X�����肷��B
		 * �����F�X���R�[�h�A�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A�X����
		 *********************************************************************/
		private string[] Get_tyakuten_GeneralDelivery(string[] sUser, OracleConnection conn2, string sTyakutenCode, string sYubinNo)
		{
			string[] sRet = new string[2];	// �߂�l
			string cmdQuery;				// SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// SQL��
			cmdQuery
				= "SELECT �X���� \n"
				+ "  FROM �b�l�P�O�X�� \n"
				+ " WHERE �X���b�c = :p_BranchCode \n"
				+ "   AND �X�֔ԍ� = :p_YubinNo \n"
				+ "   AND �x�X�~�߂e�f�P = '1' \n"
				+ "   AND �폜�e�f = '0' \n";

			wk_opOraParam = new OracleParameter[2];
			wk_opOraParam[0] = new OracleParameter("p_BranchCode", OracleDbType.Char, sTyakutenCode, ParameterDirection.Input); // �X���b�c
			wk_opOraParam[1] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // �X�֔ԍ�

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;

			// �f�[�^�擾
			if (reader.Read())
			{
				// �Y���f�[�^����
				sRet[0] = " ";							// �X�e�[�^�X
				sRet[1] = reader.GetString(0).Trim();	// �X����
			} 
			else
			{
				// �Y���f�[�^�Ȃ�
				sRet[0] = "�x�X�~�ߒ��X����F�Y������X���f�[�^�͑��݂��܂���B";
				sRet[1] = " ";
			}
			
			// �I������
			disposeReader(reader);
			reader = null;

			return sRet;
		}

		private string ���p�����ϊ�(string sData)
		{
			// �����`�F�b�N
			if(sData.Length != 3)
			{
				return "�X���R�[�h���R���ł͂���܂���B";
			}

			string wk = "";
			for(int i = 0; i < sData.Length; i++)
			{
				string sData_letter = sData.Substring(i, 1);
				switch(sData_letter)
				{
					case "�O":
						sData_letter = sData_letter.Replace("�O", "0");
						break;
					case "�P":
						sData_letter = sData_letter.Replace("�P", "1");
						break;
					case "�Q":
						sData_letter = sData_letter.Replace("�Q", "2");
						break;
					case "�R":
						sData_letter = sData_letter.Replace("�R", "3");
						break;
					case "�S":
						sData_letter = sData_letter.Replace("�S", "4");
						break;
					case "�T":
						sData_letter = sData_letter.Replace("�T", "5");
						break;
					case "�U":
						sData_letter = sData_letter.Replace("�U", "6");
						break;
					case "�V":
						sData_letter = sData_letter.Replace("�V", "7");
						break;
					case "�W":
						sData_letter = sData_letter.Replace("�W", "8");
						break;
					case "�X":
						sData_letter = sData_letter.Replace("�X", "9");
						break;
				}
				wk += sData_letter;
				sData_letter = "";
			}

			// �ϊ���̓X���R�[�h���s���ł������ꍇ
			if(wk.Length != 3)
			{
				return "�ϊ���̓X���R�[�h���A���p�����R���ł͂���܂���B�s���Ȍ`���ł��B";
			}

			return wk;
		}
// MOD 2015.07.30 BEVAS) ���{ �x�X�~�ߋ@�\�Ή� END
// MOD 2015.12.15 BEVAS) ���{ �A���֎~�G���A�@�\�Ή� START
		/*********************************************************************
		 * �z�B�s�\�G���A�`�F�b�N
		 * �@�@���͂��ꂽ�X�֔ԍ�����A
		 * �@�@�b�l�Q�P�z�B�s�\�̑��݃`�F�b�N�����{����B
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X�A���������A���b�Z�[�W�A�\���J�n���A�\���I����
		 *********************************************************************/
		[WebMethod]
		public ArrayList Check_NonDeliveryArea(string[] sUser, string sYubinNo)
		{
			logWriter(sUser, INF, "�z�B�s�\�G���A�`�F�b�N�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];  //��ɃX�e�[�^�X���i�[
			ArrayList alRet = new ArrayList();  //�߂�l
			string cmdQuery;  // SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				// SQL��
				cmdQuery
					= "SELECT ��������, ���b�Z�[�W, �\���J�n��, �\���I���� \n"
					+ "  FROM �b�l�Q�P�z�B�s�\ \n"
					+ " WHERE �X�֔ԍ� = :p_YubinNo \n"
					+ "   AND �폜�e�f = '0' \n"
					+ " ORDER BY �������� DESC, ���b�Z�[�W DESC \n";
				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_YubinNo", OracleDbType.Char, sYubinNo, ParameterDirection.Input); // �X�֔ԍ�

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// �f�[�^�擾
				while(reader.Read())
				{
					string[] sData = new string[4];
					sData[0] = reader.GetString(0).Trim(); // ��������
					sData[1] = reader.GetString(1).Trim(); // ���b�Z�[�W
					sData[2] = reader.GetString(2).Trim(); // �\���J�n��
					sData[3] = reader.GetString(3).Trim(); // �\���I����

					alRet.Add(sData); //���X�g�Ɋi�[
				}

				disposeReader(reader);
				reader = null;

				if(alRet.Count == 0)
				{
					//�Y���f�[�^�Ȃ�
					sRet[0] = "�Y���f�[�^�Ȃ�";
					alRet.Add(sRet);
				}
				else
				{
					//�Y���f�[�^����
					sRet[0] = "�Y���f�[�^����";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				// Oracle �̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				// ����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				// �I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}

			return alRet;
		}
// MOD 2015.12.15 BEVAS) ���{ �A���֎~�G���A�@�\�Ή� END
// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
		/*********************************************************************
		 * ���X�A�W��X�擾�i�Г��`�[�p�j
		 * �@�@�b�l�O�T������X�e�A�b�l�P�O�X���A�b�l�P�P�W��X���g�p����
		 * �@�@���X�A�W��X�����肷��
		 * �����F����R�[�h
		 * �ߒl�F�X�e�[�^�X�A���X�b�c�A���X���A�W��X�b�c
		 * 
		 *********************************************************************/
		private string[] Get_hatuten_HouseSlip(string[] sUser, OracleConnection conn2, string sKaiinCode)
		{
			string[] sRet = new string[4];	// �߂�l
			string cmdQuery;				// SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			// SQL��
			cmdQuery
				= " SELECT \n"
				+ "  CM05F.�X���b�c \n"
				+ " ,NVL(CM10.�X����, ' ') \n"
// MOD 2016.06.23 bevas) ���{ �Г��`�[�Ή��̃o�O�C�� START
				//�W��X�b�c�̎Q�ƕ��@��ύX
//				+ " ,NVL(CM11.�W��X�b�c, ' ') \n"
				+ " ,NVL(CM10.�W��X�b�c, '0000') \n"
// MOD 2016.06.23 bevas) ���{ �Г��`�[�Ή��̃o�O�C�� END
				+ " FROM �b�l�O�T������X�e CM05F \n"
				+ " LEFT JOIN �b�l�P�O�X�� CM10 \n"
				+ "    ON CM10.�X���b�c = CM05F.�X���b�c "
				+ "   AND CM10.�폜�e�f = '0' \n"
// MOD 2016.06.23 bevas) ���{ �Г��`�[�Ή��̃o�O�C�� START
				//�e�X�̏ꍇ�A�W��X�}�X�^�̃��R�[�h�ɍ폜�e�f�������Ă��邱�Ƃ̍l�����R��Ă����ׁA�Ή�
//				+ " LEFT JOIN �b�l�P�P�W��X CM11 \n"
//				+ "    ON CM11.�W�דX�b�c = CM10.�X���b�c "
//				+ "   AND CM11.�폜�e�f = '0' \n"
// MOD 2016.06.23 bevas) ���{ �Г��`�[�Ή��̃o�O�C�� END
				+ " WHERE CM05F.����b�c = :p_MemberCode \n"
				+ "   AND CM05F.�폜�e�f = '0' \n";

			wk_opOraParam = new OracleParameter[1];
			wk_opOraParam[0] = new OracleParameter("p_MemberCode", OracleDbType.Char, sKaiinCode, ParameterDirection.Input); // ����b�c

			reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
			wk_opOraParam = null;

			// �f�[�^�擾
			if(reader.Read())
			{
				// �Y���f�[�^����
				sRet[0] = " ";							// �X�e�[�^�X
				sRet[1] = reader.GetString(0).Trim();	// ���X�R�[�h
				sRet[2] = reader.GetString(1).Trim();	// ���X��
				sRet[3] = reader.GetString(2).Trim();	// �W��X�R�[�h
			} 
			else
			{
				// �Y���f�[�^�Ȃ�
				sRet[0] = "�Г��`���X������F�Y�����������X�f�[�^�͑��݂��܂���B"; // �X�e�[�^�X
				sRet[1] = "0000";                                                     // ���X�R�[�h
				sRet[2] = " ";                                                        // ���X��
				sRet[3] = "0000";                                                     // �W��X�R�[�h
			}
			
			// �I������
			disposeReader(reader);
			reader = null;

			return sRet;
		}

		/*********************************************************************
		 * �b�l�O�T�e���݃`�F�b�N�i�Г��`�[�p�j
		 * �@�@�Г��`���O�C�����[�U�[�ɑ΂��āA�o�דo�^�^�X�V�O��
		 * �@�@�b�l�O�T������X�e�̑��݃`�F�b�N�����{����B
		 * �����F���O�C�����[�U�[�̉���R�[�h
		 * �ߒl�F�X�e�[�^�X�A�X���R�[�h
		 * 
		 *********************************************************************/
		[WebMethod]
		public string[] CheckCM22_HouseSlip(string[] sUser, string sKaiinCD)
		{
			logWriter(sUser, INF, "�b�l�O�T�e���݃`�F�b�N�i�Г��`�[�p�j�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];	//�߂�l
			string cmdQuery;				//SQL��
			OracleDataReader reader;
			OracleParameter[] wk_opOraParam	= null;

			//�c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				//SQL��
				cmdQuery
					= "SELECT �X���b�c \n"
					+ "  FROM �b�l�O�T������X�e \n"
					+ " WHERE ����b�c = :p_MemberCode \n"
					+ "   AND �폜�e�f = '0' \n";

				wk_opOraParam = new OracleParameter[1];
				wk_opOraParam[0] = new OracleParameter("p_MemberCode", OracleDbType.Char, sKaiinCD, ParameterDirection.Input); //����b�c

				reader = CmdSelect(sUser, conn2, cmdQuery, wk_opOraParam);
				wk_opOraParam = null;

				// �f�[�^�擾
				if(reader.Read())
				{
					//�Y���f�[�^����
					sRet[0] = "����I��";                 //�X�e�[�^�X
					sRet[1] = reader.GetString(0).Trim(); //�X���b�c
				} 
				else
				{
					//�Y���f�[�^�Ȃ�
					sRet[0] = "�Y���f�[�^������܂���";   //�X�e�[�^�X
					sRet[1] = " ";                        //�X���b�c(��)
				}
			}
			catch(OracleException ex)
			{
				//Oracle�̃G���[
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = " ";
			}
			catch(Exception ex)
			{
				//����ȊO�̃G���[
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				sRet[1] = " ";
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				//�I������
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * ���X�擾�i�b�r�u�����Ď��@�\�g�p���F�Г��`�[����p�j
		 * �����F����b�c
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X����
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_hatuten2_HouseSlip(string[] sUser, string sKcode)
		{
			logWriter(sUser, INF, "���X�i�Г��`�[����p�j�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[4];

			//�c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery =
					  "SELECT CM05F.�X���b�c, NVL(CM10.�X����, ' ') \n"
					+ " FROM �b�l�O�T������X�e CM05F, \n"
					+      " �b�l�P�O�X�� CM10 \n"
					+ " WHERE CM05F.����b�c = '" + sKcode + "' \n"
					+ " AND CM05F.�폜�e�f = '0' \n"
					+ " AND CM05F.�X���b�c = CM10.�X���b�c \n"
					+ " AND CM10.�폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetString(0).Trim();
					sRet[2] = reader.GetString(1).Trim();
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �W��X�擾�i�b�r�u�����Ď��@�\�g�p���F�Г��`�[����p�j
		 * �����F����b�c
		 * �ߒl�F�X�e�[�^�X�A�W��X�b�c
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_syuuyakuten2_HouseSlip(string[] sUser, string sKcode)
		{
			logWriter(sUser, INF, "�W��X�i�Г��`�[����p�j�擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];

			//�c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				string cmdQuery = "SELECT CM10.�W��X�b�c \n"
					+ " FROM �b�l�O�T������X�e CM05F, �b�l�P�O�X�� CM10 \n"
					+ " WHERE CM05F.����b�c = '" + sKcode + "' \n"
					+ "   AND CM05F.�폜�e�f = '0' \n"
					+    "AND CM05F.�X���b�c = CM10.�X���b�c \n"
					+ "   AND CM10.�폜�e�f = '0'";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				if(reader.Read())
				{
					sRet[0] = "����I��";
					sRet[1] = reader.GetString(0).Trim();
				}
				else
				{
					sRet[0] = "�Y���f�[�^������܂���";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2016.04.08 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
	}
}
