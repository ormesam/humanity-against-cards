using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.Game {
    public static class HubProxy {
        public static async Task Call(this HubClientBase hubClient, Expression<Action<IGameHub>> expression) {
            var methodName = GetMethodName(expression);
            var methodArguments = GetMethodArguments(expression).ToList();

            Debug.WriteLine(methodName + " sent to server");

            switch (methodArguments.Count()) {
                case 0:
                    await hubClient.HubConnection.InvokeAsync(methodName);
                    break;
                case 1:
                    await hubClient.HubConnection.InvokeAsync(methodName, methodArguments[0]);
                    break;
                case 2:
                    await hubClient.HubConnection.InvokeAsync(methodName, methodArguments[0], methodArguments[1]);
                    break;
                case 3:
                    await hubClient.HubConnection.InvokeAsync(methodName, methodArguments[0], methodArguments[1], methodArguments[2]);
                    break;
                case 4:
                    await hubClient.HubConnection.InvokeAsync(methodName, methodArguments[0], methodArguments[1], methodArguments[2], methodArguments[3]);
                    break;
                case 5:
                    await hubClient.HubConnection.InvokeAsync(methodName, methodArguments[0], methodArguments[1], methodArguments[2], methodArguments[3], methodArguments[4]);
                    break;
                default:
                    throw new ArgumentException("Too many arguments");
            }
        }

        public static async Task<T> Call<T>(this HubClientBase hubClient, Expression<Func<IGameHub, Task<T>>> expression) {
            var methodName = GetMethodName(expression);
            var methodArguments = GetMethodArguments(expression).ToList();

            Debug.WriteLine(methodName + " sent to server");

            switch (methodArguments.Count()) {
                case 0:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName);
                case 1:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName, methodArguments[0]);
                case 2:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName, methodArguments[0], methodArguments[1]);
                case 3:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName, methodArguments[0], methodArguments[1], methodArguments[2]);
                case 4:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName, methodArguments[0], methodArguments[1], methodArguments[2], methodArguments[3]);
                case 5:
                    return await hubClient.HubConnection.InvokeAsync<T>(methodName, methodArguments[0], methodArguments[1], methodArguments[2], methodArguments[3], methodArguments[4]);
                default:
                    throw new ArgumentException("Too many arguments");
            }
        }

        private static string GetMethodName(LambdaExpression expression) {
            var methodCallExpression = (MethodCallExpression)expression.Body;

            return methodCallExpression.Method.Name;
        }

        private static IEnumerable<object> GetMethodArguments(LambdaExpression expression) {
            var methodCallExpression = (MethodCallExpression)expression.Body;
            var arguments = methodCallExpression.Arguments;

            foreach (var item in arguments) {
                Expression conversion = Expression.Convert(item, typeof(object));
                var argumentExpression = Expression.Lambda<Func<object>>(conversion).Compile();

                yield return argumentExpression();
            }
        }
    }
}
