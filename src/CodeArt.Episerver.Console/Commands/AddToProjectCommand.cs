using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.Console.Commands
{
    [Command(Keyword = "addtoproject")]
    public class AddToProjectCommand : IConsoleCommand, IInputCommand
    {
        private readonly ProjectRepository projectRepository;
        private readonly IContentRepository contentRepository;

        [CommandParameter]
        public string ProjectName { get; set; }

        [CommandParameter]
        public string Content { get; set; }


        private Project project;

        public AddToProjectCommand(ProjectRepository projectRepo, IContentRepository contentRepository)
        {
            projectRepository = projectRepo;
            this.contentRepository = contentRepository;
        }

        public string Execute(params string[] parameters)
        {
            if (project == null)
            {
                if (string.IsNullOrEmpty(ProjectName) && parameters.Length > 0)
                {
                    ProjectName = parameters.First();
                }
                var projects = projectRepository.List();
                project = projects.Where(p => p.Name == ProjectName).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(Content))
            {
                var c = contentRepository.Get<IContent>(ContentReference.Parse(Content));
                projectRepository.SaveItems(new ProjectItem[] { new ProjectItem(project.ID, c) });
            }
            //Find project

            //TODO: Check if items already exist in project - if so don't add


            return "Completed Add to Project";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
            if (string.IsNullOrEmpty(ProjectName) && parameters.Length > 0)
            {
                ProjectName = parameters.First();
            }
            var projects=projectRepository.List();
            project = projects.Where(p => p.Name == ProjectName).FirstOrDefault();
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            //input is IContent? Add to project
            if(output is IContent)
            {
                projectRepository.SaveItems(new ProjectItem[] { new ProjectItem(project.ID, output as IContent) });
            }
        }
    }
}