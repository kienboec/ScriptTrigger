using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ScriptTrigger.CLI.BusinessLogic.Infrastructure
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
        [SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "prepared")]
        protected T Get<T>(T field)
        {
            return field;
        }

        protected bool Set<T>(
            ref T field,
            T newValue,
            string propertyName,
            params string[] furtherPropertyNames)
        {
            return Set(
                ref field,
                newValue,
                null,
                null,
                propertyName,
                furtherPropertyNames);
        }

        protected bool Set<T>(
            ref T field,
            T newValue,
            Action<T> executeBeforePropertyChanged,
            string propertyName,
            params string[] furtherPropertyNames)
        {
            return Set(
                ref field,
                newValue,
                executeBeforePropertyChanged,
                null,
                propertyName,
                furtherPropertyNames);
        }

        protected bool Set<T>(
            ref T field,
            T newValue,
            Action<T> executeBeforePropertyChanged,
            Action<T> executeAfterPropertyChanged,
            string propertyName,
            params string[] furtherPropertyNames
            )
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;

            executeBeforePropertyChanged?.Invoke(newValue);
            DelegateRaisePropertyChanged(propertyName, furtherPropertyNames);
            executeAfterPropertyChanged?.Invoke(newValue);

            return true;
        }

        protected T Propagate<T>(T newValue, string propertyName, params string[] furtherPropertyNames)
        {
            DelegateRaisePropertyChanged(propertyName, furtherPropertyNames);
            return newValue;
        }

        protected static TEnum EnumParse<TEnum>(string value, bool ignoreCase, TEnum onErrorValue)
            where TEnum : struct
        {
            try
            {
                if (Enum.TryParse(value, ignoreCase, out TEnum enumValue))
                {
                    return enumValue;
                }

                return onErrorValue;
            }
            catch
            {
                return onErrorValue;
            }
        }

        protected void DelegateRaisePropertyChanged(string propertName, params string[] furtherPropertyNames)
        {
            OnPropertyChanged(propertName);
            foreach (var furtherPropertyName in furtherPropertyNames)
            {
                OnPropertyChanged(furtherPropertyName);
            }
        }
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
