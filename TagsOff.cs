using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class TagsOff : MonoBehaviour {

	//////////////////////////////////////
	//Script by PerguGames
	//GitHub:https://github.com/PerduGames
	//////////////////////////////////////

	 /*
	Script para encontrar as tags não utilizadas na cena 
	ou em todo projeto, recebendo seus nomes no Console ou
	criando arquivo .txt com as tags não utilizadas.

	Notes:
	- Para atualizar o arquivo .txt criado na Unity, minimize a Unity ou de um Refresh que atualiza.
	- Ouvi dizer que este por ser um recurso interno pode quebrar em futuras versões:
	"UnityEditorInternal.InternalEditorUtility.tags"
	*/

	//Bool para pesquisar dentro da cena ou do projeto inteiro
	[Header("Modo Pesquisa?")]
	[Tooltip("True = Pesquisar somente na cena. \nFalse = Pesquisar em todo projeto.")]
	[SerializeField]
	private bool ModoPesquisa;

	//Bool para gerar arquivo txt ou não
	[Header("Gerar arquivo .txt?")]
	[Tooltip("True = Gerar arquivo .txt. \nFalse = Não gerar arquivo .txt.")]
	[SerializeField]
	private bool GerarArquivo;

	//Bool para evitar tags padrão da unity
	[Header("Evitar Tags padrões da Unity?")]
	[Tooltip("True = Evitar Tags padrões da Unity. \nFalse = Não evitar Tags padrões da Unity.")]
	[SerializeField]
	private bool EvitarTagsUnity;

	//Lista para guardar tags não utilizadas
	private List<string> listaTags = new List<string>();
	//Criar array de tags padrão da Unity
	string[] tagsUnity = {"Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController"};

	// Use this for initialization
	void Awake () {
		//Pesquisar somente na cena
		if (ModoPesquisa == true) {
			for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++) {
				GameObject obj = GameObject.FindWithTag (UnityEditorInternal.InternalEditorUtility.tags [i]);
				if (obj == null) {
					//Salvar as tags não utilizadas na lista
					listaTags.Add (UnityEditorInternal.InternalEditorUtility.tags [i]);
				}
			}
			//Evitar as tags e gerar arquivo
			gerar ();
		//Pesquisar em todo projeto
		}else if(ModoPesquisa == false){
			//Criar lista de GameObject para os prefabs
			List<GameObject> listaPrefabs = new List<GameObject>();
			//Pegar FileInfo de todos prefabs do projeto
			DirectoryInfo dir = new DirectoryInfo ("Assets/");
			FileInfo[] arquivo = dir.GetFiles ("*.prefab", System.IO.SearchOption.AllDirectories);
			//Pegar todos os GameObjects 
			foreach(FileInfo i in arquivo){
				string pathCompleto = i.FullName.Replace(@"\","/");
				string pathUnity = "Assets" + pathCompleto.Replace(Application.dataPath, "");
				GameObject gobj = AssetDatabase.LoadAssetAtPath (pathUnity, typeof(GameObject)) as GameObject;
				listaPrefabs.Add (gobj);
			}

			//Colocar as tags na lista
			listaTags = UnityEditorInternal.InternalEditorUtility.tags.ToList ();

			//Pecorrer a lista das tags
			for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++) {
				int num = 0;
				//Pecorrer a lista dos objetos
				for (int j = 0; j < listaPrefabs.Count; j++) {
					//Pesquisar no objeto pai
					if (listaPrefabs [j].CompareTag (UnityEditorInternal.InternalEditorUtility.tags [i])) {
						num += 1;
					}
					//Pesquisar nos objetos filhos caso haja
					if (listaPrefabs [j].transform.childCount > 0) {
						for (int k = 0; k < listaPrefabs [j].transform.childCount; k++) {
							if(listaPrefabs [j].transform.GetChild(k).gameObject.CompareTag (UnityEditorInternal.InternalEditorUtility.tags [i])){
								num += 1;
							}
						}
					}
				}
				//Se a tag estiver sendo utilizada, remove ela da lista
				if(num != 0){
					listaTags.Remove (UnityEditorInternal.InternalEditorUtility.tags [i]);
				}
			}
			//Evitar as tags e gerar arquivo
			gerar ();
		}
	}

	//Metodo para evitar as tags e gerar arquivo caso seja true
	void gerar (){
		//Para evitar tags padrão da Unity
		if(EvitarTagsUnity == true){
			for(int i = 0; i < tagsUnity.Length; i++) {
				listaTags.Remove (tagsUnity [i]);
			}
		}
		//String para escrever no arquivo .txt
		string stg = "";
		//Mostrar as tags não utilizadas no Console
		for (int i = 0; i < listaTags.Count; i++) {
			Debug.Log (listaTags [i]);
			//Se GerarArquivo for true
			if(GerarArquivo == true){
				stg += listaTags [i] + System.Environment.NewLine;
			}
		}
		//Se GerarArquivo for true
		if (GerarArquivo == true) {
			StreamWriter writer = new StreamWriter("Assets/TagsOff.txt");
			writer.WriteLine (stg);
			writer.Close();
		}
	}
}
