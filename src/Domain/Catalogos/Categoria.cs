namespace Domain.Catalogos;

public class Categoria
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  public int Minimo { get; set; }
  public int Maximo { get; set; }
}