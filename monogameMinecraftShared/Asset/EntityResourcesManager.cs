using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftShared.Animations;

namespace monogameMinecraftShared.Asset
{
    public class ModelWithTexture
    {
        public Model model;
        public Texture texture;

        public ModelWithTexture(Model model, Texture texture)
        {
            this.model= model;
            this.texture= texture;
        }
    }

    public struct CustomModelLoadingItem
    {
        public string name;
        public string modelAssetPath;
        public string textureAssetPath;

        public CustomModelLoadingItem(string name, string modelAssetPath, string textureAssetPath)
        {
            this.modelAssetPath
                = modelAssetPath;
            this.textureAssetPath = textureAssetPath;
            this.name = name;
        }
    }

    
    public class EntityResourcesManager
    {

    
        private static EntityResourcesManager _instance;
        private static readonly object locker = new object();
        private EntityResourcesManager()
        {

        }
        public static EntityResourcesManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {

                        if (_instance == null)
                        {
                            _instance = new EntityResourcesManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public Dictionary<string, ModelWithTexture> loadedEntityModels= new Dictionary<string, ModelWithTexture>();
        public Dictionary<string, Animation> loadedEntityAnims=new Dictionary<string,Animation>();
        public Dictionary<string, SoundEffect> loadedEntitySounds = new Dictionary<string, SoundEffect>();

        public void LoadDefaultEntityModels(ContentManager cm)
        {
            ModelWithTexture zombieModel = new ModelWithTexture(cm.Load<Model>("zombiefbx"),
               cm.Load<Texture2D>("husk"));
            loadedEntityModels.TryAdd("zombie", zombieModel);
        }

        public void TryAddCustomEntityModels(ContentManager cm,params CustomModelLoadingItem[] items)
        {
            foreach (var item in items)
            {
                try
                {
                    ModelWithTexture model = new ModelWithTexture(cm.Load<Model>(item.modelAssetPath),
                        cm.Load<Texture2D>(item.textureAssetPath));
                    loadedEntityModels.Add(item.name,model);
                }
                catch(Exception ex) 
                {
                    Debug.WriteLine(ex);
                }
               
            }
        }

        public void LoadDefaultEntityAnims()
        {
            Animation zombieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) }
            }, 0.5f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
            }, 0.5f)
        }, true);
        Animation entityDieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "waist", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.4f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "waist", new AnimationTransformation(new Vector3(0f, -0.75f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
            }, 0.1f)
        }, false);
        loadedEntityAnims.TryAdd("zombieAnim", zombieAnim);
        loadedEntityAnims.TryAdd("entityDieAnim", entityDieAnim);
        }


        public void TryLoadCustomEntityAnims(params Tuple<string, Animation>[] items)
        {
            foreach (var item in items)
            {
                try
                {
                    loadedEntityAnims.TryAdd(item.Item1, item.Item2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
             
            }

           
         
        }

        public void LoadDefaultEntitySounds(ContentManager cm)
        {
            loadedEntitySounds.TryAdd("zombieHurt", cm.Load<SoundEffect>("sounds/zombiehurt"));
            loadedEntitySounds.TryAdd("zombieIdle", cm.Load<SoundEffect>("sounds/zombiesay"));
        }

        public void TryLoadCustomEntitySounds(ContentManager cm, params Tuple<string, string>[] customItems)
        {
            foreach (var item in customItems)
            {
                try
                {
                    loadedEntitySounds.TryAdd(item.Item1, cm.Load<SoundEffect>(item.Item2));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
              
            }
        }

        public void Initialize()
        {
            loadedEntityModels = new Dictionary<string, ModelWithTexture>();
            loadedEntityAnims = new Dictionary<string, Animation>();
            loadedEntitySounds=new Dictionary<string, SoundEffect>();
        }

        public void LoadAllDefaultDesources(ContentManager cm)
        {
            LoadDefaultEntityModels(cm);
            LoadDefaultEntityAnims();
            LoadDefaultEntitySounds(cm);
        }
    }
}
