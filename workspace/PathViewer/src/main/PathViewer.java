package main;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;

import com.google.gson.Gson;
import model.StatisticsForPathViewer;

/**
 * 
 * Diese Klasse stellt die Schnittstelle zum Unity-Projekt dar und erstellt eine Verbindung 
 * via TCP/IP-Verbindung als eigenen Thread.
 * 
 */
public class PathViewer {

	/**
	 * Diese Methode beinhaltet die zentrale Verarbeitung zwischen dem Unity- und Java-Projekt
	 * dar.
	 * 
	 * @param port
	 *            Der Port des Programms(Unity-Projekt).
	 */
	public static void receive(int port) {
		while (true) {

			ServerSocket serverSocket = null;

			Socket socket = null;
			InputStream in = null;
			OutputStream out = null;

			try {

				serverSocket = new ServerSocket(port);

				socket = serverSocket.accept();

				in = socket.getInputStream();

				out = socket.getOutputStream();

				String clientSentence;

				BufferedReader inFromClient = new BufferedReader(new InputStreamReader(in));
				clientSentence = inFromClient.readLine();
				//System.out.println("clientSentence: " + clientSentence);
				Gson gson = new Gson();

				StatisticsForPathViewer statisticsForPathViewer = 
						gson.fromJson(clientSentence, StatisticsForPathViewer.class);
				System.out.println("PathViewer Received in java: " + statisticsForPathViewer);
				writeToFile(statisticsForPathViewer.toString());
							
				

			} catch (IOException exc) {
				System.out.println(exc.getMessage());
				writeToFile(exc.getMessage());
			} finally {
				try {
					out.close();
					in.close();
					socket.close();
					serverSocket.close();
					break;
				} catch (IOException e) {
					e.printStackTrace();
				}

			}

		}

	}

	/**
	 * Diese Methode speichert die erhaltenen Informationen des Unity-Projekt in einer Text-Datei(Visab).
	 * 
	 * @param text
	 *            Der Text, welcher abgespeichert werden soll.
	 */
	public static void writeToFile(String text) {
		FileWriter fw = null;
		try {
			File file = new File(System.getProperty("user.dir"), "PathViewerData.visab");
//			if (file.exists()) {
//				
//				String name = file.getName();
//				char nrChar = name.charAt(14);
//				int nrInt = Character.getNumericValue(nrChar); 				
//				
//				nrInt++;
//				
//				String nrString = Integer.toString(nrInt);
//				
//				file = new File(System.getProperty("user.dir"), "PathViewerData" + nrString + ".txt");	
//				
//				System.out.println(file.getName());
//				
//				
//			}
			fw = new FileWriter(file, true);
			fw.write(text);
			fw.write("\r\n");

		} catch (IOException e) {
			e.printStackTrace();
		} finally {

			try {
				fw.flush();
				fw.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}

	/**
	 * Initialisierung und startet der TCP/IP-Verbindung.
	 * 
	 * @param args
	 *            Der Parameter beinhaltet den Port des Programms(Unity-Projekt).
	 */
	public static void main(String[] args) {
		int port;

		if (args.length == 0) {
			port = 5557;
		} else {
			port = Integer.parseInt(args[0]);
		}
		System.out.println("Starting server on port " + port);

		new Thread(new Runnable() {

			@Override
			public void run() {
				while (true) {
					receive(port);
				}
			}
		}).start();
	}
}


