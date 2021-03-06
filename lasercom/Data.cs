﻿using System;

namespace lasercom
{
    /// <summary>
    /// Provides methods for the manipulation of numerical data,
    /// including array math, statistics and optical calculations.
    /// </summary>
    public static class Data
    {

        public static void NormalizeArray(int[] arr, int maxval)
        {
            int max = AbsMax(arr);
            for (int i = 0; i < arr.Length; i++)
            {
                int denom = max * maxval;
                arr[i] = denom == 0 ? 0 : arr[i] / denom;
            }
        }

        public static void Accumulate(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] += b[i];
        }

        public static void Accumulate(double[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] += b[i];
        }

        public static void Accumulate(double[] a, double[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] += b[i];
        }

        public static void Dissipate(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] -= b[i];
        }

        public static void Dissipate(double[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] -= b[i];
        }

        public static void Dissipate(double[] a, double[] b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] -= b[i];
        }

        public static int[] DummySpectrum(double t)
        {
            int[] data = new int[1024];
            return data;
        }

        public static int AbsMax(int[] arr)
        {
            int curmax = int.MinValue;
            foreach (int i in arr)
                curmax = Math.Abs(arr[i]) > curmax ? Math.Abs(arr[i]) : curmax;
            return curmax;
        }

        public static void DivideArray(int[] arr, int N)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] /= N;
            }
        }

        public static void DivideArray(double[] arr, double N)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] /= N;
            }
        }

        public static void MultiplyArray(int[] arr, int N)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= N;
            }
        }

        public static void MultiplyArray(double[] arr, double N)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] *= N;
            }
        }

        /// <summary>
        /// Computes OD from Sample and Blank counts, subtracting Dark counts from both.
        /// </summary>
        /// <param name="Sample"></param>
        /// <param name="Blank"></param>
        /// <param name="Dark"></param>
        /// <returns>Optical density</returns>
        public static double[] OpticalDensity(int[] Sample, int[] Blank, int[] Dark)
        {
            double[] OD = new double[Sample.Length];
            for (int i = 0; i < OD.Length; i++)
                OD[i] = -Math.Log10((double)(Sample[i] - Dark[i]) / (double)(Blank[i] - Dark[i]));
            return OD;
        }

        /// <summary>
        /// Computes OD from Sample and Blank counts. Assumes Dark has been applied or is zero.
        /// </summary>
        /// <param name="Sample"></param>
        /// <param name="Blank"></param>
        /// <returns>Optical density</returns>
        public static double[] OpticalDensity(int[] Sample, int[] Blank)
        {
            double[] OD = new double[Sample.Length];
            for (int i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10((double)(Sample[i]) / (double)(Blank[i]));
            return OD;
        }

        /// <summary>
        /// Computes delta OD from Ground, Trans and Dark counts.
        /// </summary>
        /// <param name="Ground"></param>
        /// <param name="Trans"></param>
        /// <param name="Dark"></param>
        /// <returns>Delta OD</returns>
        public static double[] DeltaOD(int[] Ground, int[] Trans, int[] Dark)
        {
            double[] OD = new double[Ground.Length];
            for (int i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10((double)(Trans[i] - Dark[i]) / (double)(Ground[i] - Dark[i]));
            return OD;
        }

        /// <summary>
        /// Computes delta OD from Ground and Trans. Assumes Dark has been applied or is zero.
        /// </summary>
        /// <param name="Ground"></param>
        /// <param name="Trans"></param>
        /// <returns>Delta OD</returns>
        public static double[] DeltaOD(int[] Ground, int[] Trans)
        {
            double[] OD = new double[Ground.Length];
            for (int i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10((double)(Trans[i]) / (double)(Ground[i]));
            return OD;
        }

        public static double[] DeltaOD(double[] Ground, double[] Trans)
        {
            double[] OD = new double[Ground.Length];
            for (int i = 0; i < OD.Length; i++)
                //OD[i] = Math.Log10((Trans[i]) / (Ground[i]));
                OD[i] = Math.Log10(Trans[i]) - Math.Log10(Ground[i]);
            return OD;
        }

        public static double[] Gaussian(int n, double scale, double mean, double sigma)
        {
            double[] g = new double[n];
            for (int i = 0; i < g.Length; i++)
            {
                double x = (double)i * 1024 / n;
                g[i] = scale * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(sigma, 2)));
            }
            return g;
        }

        public static int[] Gaussian(int n, int scale, double mean, double sigma)
        {
            int[] g = new int[n];
            for (int i = 0; i < g.Length; i++)
            {
                double x = (double)i * 1024 / n;
                g[i] = (int)( scale * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(sigma, 2))) );
            }
            return g;
        }

        public static double[] Calibrate(double[] channel, double slope, double intercept)
        {
            double[] cal = new double[channel.Length];
            for (int i = 0; i < channel.Length; i++)
            {
                cal[i] = slope * channel[i] + intercept;
            }
            return cal;
        }

        public static double[] Calibrate(int n, double slope, double intercept)
        {
            double[] cal = new double[n];
            for (int i = 0; i < n; i++)
            {
                cal[i] = slope * (i+1) + intercept;
            }
            return cal;
        }

        /// <summary>
        /// Linear least squares fit of variables in x to variables in y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Tuple containing slope, y-intercept and R^2</returns>
        public static Tuple<double,double,double> LinearLeastSquares(double[] x, double[] y)
        {
            double n = x.Length;
            double xysum = 0;
            double xsum = 0;
            double xsqsum = 0;
            double ysum = 0;
            double ysqsum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                xysum += x[i] * y[i];
                xsum += x[i];
                xsqsum += Math.Pow(x[i], 2);
                ysum += y[i];
                ysqsum += Math.Pow(y[i], 2);
            }
            double xyhat = xysum / n;
            double xhat = xsum / n;
            double yhat = ysum / n;
            double xsqhat = xsqsum / n;
            double ysqhat = ysqsum / n;

            double cov = xyhat - xhat * yhat;
            double xvar = xsqhat - Math.Pow(xhat, 2);
            double yvar = ysqhat - Math.Pow(yhat, 2);

            double slope = cov / xvar;
            double yint = yhat - slope * xhat;
            double rsq = Math.Pow( cov / Math.Sqrt(xvar * yvar), 2);

            return Tuple.Create<double,double,double>(slope, yint, rsq);
        }

        public static int[] Uniform(int n, int scale)
        {
            Random R = new Random();
            int[] A = new int[n];
            //for (int i = 0; i < n; i++) A[i] = (int)Math.Round(R.NextDouble() * (double)scale);
            for (int i = 0; i < n; i++) A[i] = R.Next(scale);
            return A;
        }

        public static double[] Uniform(int n, double scale)
        {
            Random R = new Random();
            double[] A = new double[n];
            for (int i = 0; i < n; i++) A[i] = Math.Round(R.NextDouble() * scale);
            return A;
        }

        public static void CumulativeMovingAverage(double[] CMA, double[] X, int n)
        {
            for (int i = 0; i < CMA.Length; i++)
                CMA[i] = (X[i] + n * CMA[i]) / (n + 1);
        }
        
        public static void CumulativeMovingAverage(double[] CMA, int[] X, int n)
        {
            for (int i = 0; i < CMA.Length; i++)
                CMA[i] = (X[i] + n * CMA[i]) / (n + 1);
        }
    }
}
