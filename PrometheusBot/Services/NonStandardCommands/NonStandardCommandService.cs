using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Modules;
using PrometheusBot.Extensions;
using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace PrometheusBot.Services.NonStandardCommands
{
    public class NonStandardCommandService
    {
        private static readonly Type moduleBase = Assembly.GetAssembly(typeof(ModuleBase<>)).GetType("Discord.Commands.IModuleBase");

        private RunMode _runMode = RunMode.Sync;
        private List<MethodInfo> _methods;

        public event Func<Optional<CommandInfo>, ICommandContext, IResult, Task> CommandExecuted;

        public NonStandardCommandService()
        {
            
        }

        public NonStandardCommandService(RunMode runMode)
        {
            _runMode = runMode;
        }

        public Task AddModulesAsync(Assembly assembly)
        {
            _methods = assembly.GetTypes()
                .Where(t => t.GetInterface(moduleBase.Name) is not null)
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute(typeof(NonStandardCommandAttribute)) is not null)
                .Where(m => m.ReturnType == typeof(Task) || m.ReturnType == typeof(Task<RuntimeResult>))
                .Where(m => m.GetGenericArguments().Length == 0)
                .ToList();

            _methods.Sort(new Comparer());

            return Task.CompletedTask;
        }
        public async Task<IResult> ExecuteAsync(ICommandContext context, IServiceProvider services = null)
        {
            var task = Task.Run(() => ExecuteAsyncPrivate(context, services));
            _ = task.ContinueWith(task => OnCommandExecuted(context, task));
            if (_runMode == RunMode.Sync)
                return await task;
            return new NonStandardCommandResult();
        }
        private IResult ExecuteAsyncPrivate(ICommandContext context, IServiceProvider services)
        {
            var method = GetFirstValid(context, services);

            if (method is null) return null;

            Type declaringType = method.DeclaringType;

            object module = ActivatorUtilities.CreateInstance(services, declaringType);

            var setContext = moduleBase.GetMethod("SetContext", new[] { typeof(ICommandContext) });
            setContext.Invoke(module, new[] { context });

            Task task = (Task)method.Invoke(module, Array.Empty<object>());

            try
            {
                task.Wait();
                if (task is Task<IResult> resultTask)
                {
                    return resultTask.Result;
                }
                return new NonStandardCommandResult();
            }
            catch (Exception ex)
            {
                return new NonStandardCommandResult(CommandError.Exception, ex.Message);
            }
        }
        private MethodInfo GetFirstValid(ICommandContext context, IServiceProvider services)
        {
            foreach (MethodInfo method in _methods)
            {
                bool valid = false;
                try
                {
                    valid = method.GetCustomAttribute<NonStandardCommandAttribute>().Validate(context, services);
                }
                catch { }
                if (valid) return method;
            }
            return null;
        }
        public void OnCommandExecuted(ICommandContext context, Task<IResult> task)
        {
            IResult result = task.Result;

            if (result is null) return;

            CommandExecuted?.Invoke(new Optional<CommandInfo>(), context, result);
        }

        class Comparer : IComparer<MethodInfo>
        {
            public int Compare(MethodInfo x, MethodInfo y)
            {
                int xPriority = x.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0;
                int yPriority = y.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0;

                return yPriority - xPriority;
            }
        }
    }
}
