using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lab1_Lissajous.Commands;
using Lab1_Lissajous.Models;

namespace Lab1_Lissajous.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private double ax = 1;
    private double ay = 1;
    private double fx = 1;
    private double fy = 1;
    private double phaseX;
    private double phaseY;
    private double dt = 0.005;

    private PointCollection curvePoints = new();
    private string statusMessage = "Готово до побудови.";
    private string ratioText = "1 : 1";

    public MainViewModel()
    {
        BuildCommand = new RelayCommand(_ => Build());
        PresetCommand = new RelayCommand(parameter => SetPreset(parameter?.ToString()));
        Build();
    }

    public double Ax
    {
        get => ax;
        set => SetField(ref ax, value);
    }

    public double Ay
    {
        get => ay;
        set => SetField(ref ay, value);
    }

    public double Fx
    {
        get => fx;
        set => SetField(ref fx, value);
    }

    public double Fy
    {
        get => fy;
        set => SetField(ref fy, value);
    }

    public double PhaseX
    {
        get => phaseX;
        set => SetField(ref phaseX, value);
    }

    public double PhaseY
    {
        get => phaseY;
        set
        {
            if (SetField(ref phaseY, value))
                OnPropertyChanged(nameof(PhaseYText));
        }
    }

    public string PhaseYText => $"{PhaseY:F0}°";

    public double Dt
    {
        get => dt;
        set => SetField(ref dt, value);
    }

    public PointCollection CurvePoints
    {
        get => curvePoints;
        private set => SetField(ref curvePoints, value);
    }

    public string StatusMessage
    {
        get => statusMessage;
        private set => SetField(ref statusMessage, value);
    }

    public string RatioText
    {
        get => ratioText;
        private set => SetField(ref ratioText, value);
    }

    public ICommand BuildCommand { get; }
    public ICommand PresetCommand { get; }

    private void Build()
    {
        if (Ax <= 0 || Ay <= 0)
        {
            StatusMessage = "Помилка: амплітуди Ax та Ay мають бути більшими за 0.";
            return;
        }

        if (Fx <= 0 || Fy <= 0)
        {
            StatusMessage = "Помилка: частоти fx та fy мають бути більшими за 0.";
            return;
        }

        if (Dt <= 0)
        {
            StatusMessage = "Помилка: крок часу dt має бути більшим за 0.";
            return;
        }

        double fastestPeriod = 1.0 / Math.Max(Fx, Fy);

        if (Dt > fastestPeriod / 20.0)
        {
            StatusMessage = $"Помилка: dt завеликий. Для щонайменше 20 точок на найшвидший період dt має бути не більше {fastestPeriod / 20.0:F4} с.";
            return;
        }

        List<ModelPoint> modelPoints = LissajousModel.Generate(
            Ax, Ay, Fx, Fy, PhaseX, PhaseY, Dt);

        CurvePoints = ToScreenPoints(modelPoints, 800, 560);
        RatioText = $"{Fx:G3} : {Fy:G3}";
        StatusMessage = $"Побудовано {CurvePoints.Count} точок. Співвідношення частот: {RatioText}.";
    }

    private PointCollection ToScreenPoints(
        List<ModelPoint> points,
        double width,
        double height)
    {
        double maxX = Ax;
        double maxY = Ay;

        double scaleX = (width * 0.46) / maxX;
        double scaleY = (height * 0.46) / maxY;
        double scale = Math.Min(scaleX, scaleY);

        double centerX = width / 2.0;
        double centerY = height / 2.0;

        var result = new PointCollection();

        foreach (ModelPoint point in points)
        {
            double screenX = centerX + point.X * scale;
            double screenY = centerY - point.Y * scale;
            result.Add(new Point(screenX, screenY));
        }

        return result;
    }

    private void SetPreset(string? preset)
    {
        switch (preset)
        {
            case "1:1":
                Fx = 1;
                Fy = 1;
                break;

            case "1:2":
                Fx = 1;
                Fy = 2;
                break;

            case "2:3":
                Fx = 2;
                Fy = 3;
                break;

            case "3:4":
                Fx = 3;
                Fy = 4;
                break;
        }

        Build();
    }

    private bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
