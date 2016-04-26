namespace DvrSniffer
{
    internal class DvrLoginPacket : DvrPacket
    {
        private const int StringLength = 36;
        private const int UserNameOffset = 0x20;
        private const int PasswordOffset = 0x44;
        private const int MachineNameOffset = 0x68;

        public override DvrPacketType PacketType => DvrPacketType.Login;

        public string UserName
        {
            get { return GetString(UserNameOffset); }
            set
            {
                ClearBytes(UserNameOffset, StringLength);
                SetString(value, UserNameOffset);
            }
        }

        public string Password
        {
            get { return GetString(PasswordOffset); }
            set
            {
                ClearBytes(PasswordOffset, StringLength);
                SetString(value, PasswordOffset);
            }
        }

        public string MachineName
        {
            get { return GetString(MachineNameOffset); }
            set
            {
                ClearBytes(MachineNameOffset, StringLength);
                SetString(value, MachineNameOffset);
            }
        }

        public DvrLoginPacket()
            : base(136)
        {
            // unknown bytes
            SetBytes(new byte[] { 0x18, 0x58, 0x2A, 0x15, 0x3E, 0x0A, 0x3C, 0x63, 0x78 }, 0x0C);
            SetByte(0x03, 0x18);
            SetBytes(new byte[] { 0x40, 0x8D, 0x5C, 0x1E, 0xD9, 0x5F }, 0x84);
            SetByte(0x04, 0x8C);
        }
    }
}
