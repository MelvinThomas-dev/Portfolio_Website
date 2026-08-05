import { useRef } from 'react';
import { Canvas, useFrame } from '@react-three/fiber';
import { Float } from '@react-three/drei';

function CodeBlock({ position, rotation, color, scale = 1 }) {
  const meshRef = useRef(null);

  useFrame((state) => {
    if (!meshRef.current) return;
    meshRef.current.rotation.y = rotation[1] + Math.sin(state.clock.elapsedTime * 0.35) * 0.08;
  });

  return (
    <Float speed={1.2} rotationIntensity={0.15} floatIntensity={0.35}>
      <mesh ref={meshRef} position={position} rotation={rotation} scale={scale}>
        <boxGeometry args={[1.4, 0.85, 0.08]} />
        <meshStandardMaterial color={color} transparent opacity={0.82} metalness={0.15} roughness={0.35} />
      </mesh>
    </Float>
  );
}

function LaptopSilhouette() {
  const groupRef = useRef(null);

  useFrame((state) => {
    if (!groupRef.current) return;
    groupRef.current.rotation.y = Math.sin(state.clock.elapsedTime * 0.25) * 0.12;
  });

  return (
    <group ref={groupRef} position={[0, -0.15, 0]}>
      <mesh position={[0, 0.05, 0]}>
        <boxGeometry args={[2.2, 1.35, 0.06]} />
        <meshStandardMaterial color="#1a9bb8" transparent opacity={0.55} metalness={0.2} roughness={0.4} />
      </mesh>
      <mesh position={[0, -0.72, 0.18]} rotation={[-0.22, 0, 0]}>
        <boxGeometry args={[2.5, 0.08, 1.1]} />
        <meshStandardMaterial color="#0d6e8a" transparent opacity={0.65} metalness={0.25} roughness={0.45} />
      </mesh>
    </group>
  );
}

function SceneContent() {
  return (
    <>
      <ambientLight intensity={0.65} />
      <directionalLight position={[4, 6, 5]} intensity={0.85} />
      <LaptopSilhouette />
      <CodeBlock position={[-1.8, 1.1, 0.4]} rotation={[0.1, 0.45, 0.05]} color="#5dd4e3" scale={0.75} />
      <CodeBlock position={[1.9, 0.85, -0.2]} rotation={[-0.08, -0.55, 0.04]} color="#94e2ed" scale={0.65} />
      <CodeBlock position={[0.6, -1.35, 0.6]} rotation={[0.12, 0.25, -0.06]} color="#2ec4d6" scale={0.55} />
    </>
  );
}

export default function HeroScene3D() {
  return (
    <Canvas
      className="hero__canvas"
      camera={{ position: [0, 0.4, 5.5], fov: 42 }}
      dpr={[1, 1.5]}
      gl={{ antialias: true, alpha: true }}
    >
      <SceneContent />
    </Canvas>
  );
}
