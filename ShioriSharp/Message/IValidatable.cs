namespace ShioriSharp.Message {
    public interface IValidatable<T> {
        public bool Valid { get; }
        public T Validate();
    }
}

