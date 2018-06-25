using System;
using Microsoft.Extensions.Caching.Memory;

namespace ONS.AuthProvider.OAuth.Util
{   
    ///<summary>Classe para gerenciamento de cache.</summary>
    public class CacheManager 
    { 
        private static ICache _managedCache;

        ///<summary>Método para fazer inicialização e configuração de cache.</summary>
        ///<param name="memoryCache">Instância de memória para ser gerenciada.</param>
        ///<param name="memoryOptions">Opções de configuração da memória.</param>
        public static void Init(IMemoryCache memoryCache, MemoryCacheEntryOptions memoryOptions = null) {
            _managedCache = new DelegateLocalCache(memoryCache, memoryOptions);
        }

        ///<summary>Método que retorna a cache gerenciada pelo sistema.</summary>
        ///<returns>Cache gerenciada.</returns>
        public static ICache Cache {
            get {
                return _managedCache;
            }
        }
    }

    ///<summary>Interface que define os métodos de acesso a cache.</summary>
    public interface ICache {

        ///<summary>Método para obter o dado da cache.</summary>
        ///<param name="key">Chave da cache para o valor da cache.</param>
        ///<returns>Valor da cache armazenado.</returns>
        T Get<T>(object key);
        
        ///<summary>Método para adiciona um item na cache.</summary>
        ///<param name="key">Chave de identificação do item na cache.</param>
        ///<param name="value">Dado para ser armazenado na cache.</param>
        void Set<T>(object key, T value);

        ///<summary>Método utilizado para remover item da cache.</summary>
        ///<param name="key">Chave de identificação do item na cache.</param>
        void Remove(object key);

        ///<summary>Método atribuir configurações da cache.</summary>
        ///<param name="options">Opções de configuração da cache.</param>
        void SetOptions(object options);

    }

    public class DelegateLocalCache : ICache {

        private IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions _memoryOptions;

        public void SetOptions(object options) {
            _memoryOptions = (MemoryCacheEntryOptions) options;
        }

        public DelegateLocalCache(IMemoryCache memoryCache, MemoryCacheEntryOptions memoryOptions) {
            _memoryCache = memoryCache;
            _memoryOptions = memoryOptions != null ? memoryOptions: new MemoryCacheEntryOptions();
        }

        public T Get<T>(object key) {
            return (T)_memoryCache.Get(key);
        }

        public void Set<T>(object key, T value) {
            _memoryCache.Set(key, value, _memoryOptions);
        }

        public void Remove(object key) {
            _memoryCache.Remove(key);
        }
    }
}