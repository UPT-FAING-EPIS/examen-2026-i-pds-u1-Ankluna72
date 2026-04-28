interface AvatarProps {
  name: string;
  size?: number;
}

function getInitials(name: string) {
  return name.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2);
}

export default function Avatar({ name, size = 36 }: AvatarProps) {
  return (
    <div
      className="avatar"
      style={{ width: size, height: size, fontSize: size * 0.36 }}
      title={name}
    >
      {getInitials(name)}
    </div>
  );
}
