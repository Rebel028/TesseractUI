using System;
using System.Collections.Generic;
using TesseractWrappers.Models;

namespace TesseractWrappers
{
    public class PixHandlersManager
    {
        public List<PixHandler> Handlers { get; private set; } = new List<PixHandler>();

        private int _idSequence = 1;

        public PixHandlersManager(bool createEmpty = false)
        {
            if (!createEmpty)
            {
                CreateDefaultHandlers();
            }
        }

        private void CreateDefaultHandlers()
        {
            AddHandler(new DeskewHandler());
            AddHandler(new GrayscaleHandler());
            //AddHandler(new RemoveLinesHandler());
        }

        public void AddDefaultHandler<T>() where T: PixHandler
        {
            T obj = Activator.CreateInstance<T>();
            if (obj is PixHandler handler)
            {
                AddHandler(handler);
            }
        }

        public void AddHandler(PixHandler handler)
        {
            handler.Id = _idSequence++;
            this.Handlers.Add(handler);
        }

        public void RemoveHandler(int id)
        {
            this.Handlers.RemoveAll(handler => handler.Id == id);
        }
    }
}